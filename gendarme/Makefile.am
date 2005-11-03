SUBDIRS = man framework console rules
bin_SCRIPTS = gendarme
EXTRA_DIST = gendarme.in AssemblyInfo.cs.in MIT.X11
DISTCLEANFILES = AssemblyInfo.cs

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g"       \
	    -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
	    -e "s|\@bindir\@|$(bindir)|g"		\
	    -e "s|\@MONO\@|$(MONO)|g" 

$(bin_SCRIPTS): $(srcdir)/gendarme.in Makefile
	$(REWRITE) $(srcdir)/gendarme.in > $@.tmp
	mv $@.tmp $@

CLEANFILES = \
	gendarme

DISTCLEANFILES = \
	configure \
	config.guess \
	config.sub \
	Makefile.in \
	install-sh \
	aclocal.m4 \
	INSTALL \
	COPYING \
	missing
