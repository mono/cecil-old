SUBDIRS= Gendarme.Rules.Performance Gendarme.Rules.Security
EXTRA_DIST = rules.xml
DISTCLEANFILES = Makefile.in

install-data-hook:
	$(INSTALL) -c -m 0644 rules.xml $(DESTDIR)$(pkglibdir);

uninstall-hook:
	rm -f $(DESTDIR)$(pkglibdir)/`basename rules.xml`;
