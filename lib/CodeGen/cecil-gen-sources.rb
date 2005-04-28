# /cecil/lib/ruby CodeGen/cecil-gen-sources.dll > Mono.Cecil.dll.sources
[ "Mono.Cecil", "Mono.Cecil.Binary",
    "Mono.Cecil.Metadata", "Mono.Cecil.Cil",
    "Mono.Cecil.Implem", "Mono.Cecil.Signatures" ].each { |dir|
    
    Dir.foreach(dir) { |file|
        $stdout.print("./#{dir}/#{file}\n") if file[(file.length - 3)..file.length] == ".cs"
    }
}
