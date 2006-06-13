SUBDIRS= Gendarme.Rules.Performance Gendarme.Rules.Security \
	 Gendarme.Rules.Correctness Gendarme.Rules.Concurrency \
	 Gendarme.Rules.Portability

EXTRA_DIST = rules.xml
DISTCLEANFILES = Makefile.in

install-data-hook:
	$(INSTALL) -c -m 0644 rules.xml $(DESTDIR)$(pkglibdir);

uninstall-hook:
	rm -f $(DESTDIR)$(pkglibdir)/`basename rules.xml`;

test:
	for ASM in $(SUBDIRS); do \
		cd $$ASM;	\
		make test;	\
		cd ..;		\
	done;

run-test:
	for ASM in $(SUBDIRS); do \
		cd $$ASM;	\
		make run-test;	\
		cd ..;		\
	done;

self-test: all
	for ASM in $(SUBDIRS); do \
		cd $$ASM;	\
		make self-test;	\
		cd ..;		\
	done;
