bin_SCRIPTS = permview
EXTRA_DIST = permview.in MIT.X11
DISTCLEANFILES = 

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g"       \
	    -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
	    -e "s|\@bindir\@|$(bindir)|g"		\
	    -e "s|\@MONO\@|$(MONO)|g" 

$(bin_SCRIPTS): $(srcdir)/permview.in Makefile
	$(REWRITE) $(srcdir)/permview.in > $@.tmp
	mv $@.tmp $@

CLEANFILES = \
	permview

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
