permview_SCRIPTS = permview.exe
permviewdir = $(prefix)/lib/permview

scriptdir = $(bindir)
script_SCRIPTS = permview

EXTRA_DIST = permview.cs permview.in MIT.X11

REWRITE = \
	sed -e "s|\@permview_exe\@|$(permviewdir)/$(permview_SCRIPTS)|g"       \
	    -e "s|\@MONO\@|$(MONO)|g" 

$(script_SCRIPTS): $(srcdir)/permview.in Makefile
	$(REWRITE) $(srcdir)/permview.in > $@.tmp
	mv $@.tmp $@

CLEANFILES = \
	permview \
	permview.exe \
	permview.exe.mdb

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

all: permview.exe

permview.exe: permview.cs
	$(MCS) -target:exe -pkg:mono-cecil -out:$@ permview.cs

install-data-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;
