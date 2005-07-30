#!/usr/bin/env ruby
# extern libraries used by code generator

require 'eruby'
require 'rexml/document'

# our library

require 'cecil-gen-types'

# time to generate code now

$headers = Hash.new
$types = Hash.new
$tables = Array.new
$colls = Array.new
$coded_indexes = Array.new
$ops = Array.new

doc = REXML::Document.new(File.open("cecil.xml"))

doc.root.each_element("/cecil/types//type") { |node|
	type = Cecil::Type.new(
		node.attribute("name").value,
		node.attribute("size").value.to_i,
		(node.attribute("underlying").nil? ? nil : node.attribute("underlying").value))
	$types[type.name] = type
}

doc.root.each_element("/cecil/headers//header") { |node|
	header = Cecil::Header.new(node.attribute("name").value)
	node.each_element("field") { |fi|
		header.add_field(Cecil::Field.new(
			fi.attribute("name").value,
			$types[fi.attribute("type").value],
			(fi.attribute("default").nil? ? nil : fi.attribute("default").value)))
	}
	$headers[header.name] = header
}

doc.root.each_element("/cecil/metadata/tables//table") { |node|
	table = Cecil::Table.new(node.attribute("name").value,
		node.attribute("rid").value,
		node.attribute("requires").nil? ? nil : node.attribute("requires").value)
	node.each_element("column") { |col|
		column = Cecil::Column.new(col.attribute("name").value,
			$types[col.attribute("type").value],
			col.attribute("target").nil? ? nil : col.attribute("target").value)
		table.add_column(column)
	}
	$tables.push(table)
}

$tables.sort!() { |a, b|
	a.name <=> b.name
}

doc.root.each_element("/cecil/metadata/codedindexes//codedindex") { |node|
	ci = Cecil::CodedIndex.new(node.attribute("name").value,
		node.attribute("size").value,
		node.attribute("requires").nil? ? nil : node.attribute("requires").value)
	node.each_element("table") { |table|
		ci.add_table(table.attribute("name").value, table.attribute("tag").value)
	}
	$coded_indexes.push(ci)
}

doc.root.each_element("/cecil/metadata/opcodes//opcode") { |node|
	$ops.push(Cecil::OpCode.new(node.attribute("name").value, node.attribute("op1").value,
		node.attribute("op2").value, node.attribute("flowcontrol").value,
		node.attribute("opcodetype").value, node.attribute("operandtype").value,
		node.attribute("stackbehaviourpop").value, node.attribute("stackbehaviourpush").value,
		node.attribute("requires").nil? ? nil : node.attribute("requires").value))
}

$ops.sort!() { |a, b|
	if a.op1 == b.op1
		eval(a.op2) <=> eval(b.op2)
	elsif a.op1 == "0xff"
		-1
	else
		1	
	end
}

doc.root.each_element("/cecil/collections//collection") { |node|
	$colls.push(Cecil::Collection.new(node.attribute("type").value, node.attribute("container").value,
		(node.attribute("visit").nil? ? nil : node.attribute("visit").value),
		(node.attribute("name").nil? ? nil : node.attribute("name").value),
		(node.attribute("lazyload").nil? ? false : node.attribute("lazyload").value == "true"),
		(node.attribute("pathtoloader").nil? ? nil : node.attribute("pathtoloader").value),
		node.attribute("target").value,
		(node.attribute("indexed").nil? ? false : node.attribute("indexed").value == "true"),
		(node.attribute("usecontainerinterface").nil? ? false : node.attribute("usecontainerinterface").value == "true")))
}

$compiler = ERuby::Compiler.new()

def cecil_compile(file, template)

	if (!File.exists?(file))

		File.open(file, File::CREAT|File::WRONLY) { |cur_file|
			$> = cur_file
			File.open(template) { |tpl|
				eval($compiler.compile_file(tpl))
			}
			$> = STDOUT
		}

		puts("#Created: #{file}")

	else

		ext = ".tmp"

		File.open(file + ext, File::CREAT|File::WRONLY) { |temp_file|
			$> = temp_file
			File.open(template) { |tpl|
				eval($compiler.compile_file(tpl))
			}
			$> = STDOUT
		}

		save = Array.new

		[file, file + ext].each { |fileloc|
			File.open(fileloc, File::RDONLY) { |f|
				buf = f.readlines
				buf.each { |line|
					line.chomp!()
				}
				buf = buf.join
				buf = buf[buf.index("*/"), buf.length]
				save.push(buf)
			}
		}

		if (save[0] != save[1]) then
			File.delete(file) if File.exists?(file)
			File.rename(file + ext, file)
			puts("#Modified: #{file}")
		else
			File.delete(file + ext)
		end
	end
end

[ "PEFileHeader.cs", "PEOptionalHeader.cs", "Section.cs",
  "CLIHeader.cs", "ImageReader.cs", "ImageWriter.cs" ].each { |file|
	cecil_compile(fullpath = "../Mono.Cecil.Binary/" + file, "./templates/" + file)
}

$tables.each { |table|
	$cur_table = table
	filename = "../Mono.Cecil.Metadata/" + table.name + ".cs"
	cecil_compile(filename, "./templates/Table.cs")
}
$cur_table = nil

[ "MetadataTableReader.cs", "MetadataRowReader.cs",
  "MetadataTableWriter.cs", "MetadataRowWriter.cs",
  "CodedIndex.cs", "Utilities.cs" ].each { |file|
	cecil_compile("../Mono.Cecil.Metadata/" + file, "./templates/" + file)
}

cecil_compile("../Mono.Cecil.Metadata/IMetadataVisitor.cs", "./templates/IMetadataVisitor.cs")

cecil_compile("../Mono.Cecil.Cil/OpCodes.cs", "./templates/OpCodes.cs")

$colls.each { |coll|
	$cur_coll = coll
	files = [ "../#{coll.target}/" + coll.intf + ".cs",
		"../Mono.Cecil.Implem/" + coll.name + ".cs" ]
	type = coll.indexed ? "Indexed" : "Named"
	templates = [ "./templates/I#{type}Collection.cs", "./templates/#{type}CollectionImplem.cs" ]
	templates[1] = "./templates/Lz#{type}CollectionImplem.cs" if coll.lazyload
	files.each_with_index { |file, i|

		cecil_compile(file, templates[i])
	}
}
$cur_coll = nil

