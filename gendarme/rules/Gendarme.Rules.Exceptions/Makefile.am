SUBDIRS=Test

security_rulesdir=$(pkglibdir)
security_rules_SCRIPTS = Gendarme.Rules.Exceptions.dll
EXTRA_DIST = $(exceptions_rules_sources) $(exceptions_rules_sources_in) Gendarme.Rules.Exceptions.xml \
	Gendarme.Rules.Exceptions.mdp Gendarme.Rules.Exceptions.csproj
CLEANFILES = Gendarme.Rules.Exceptions.dll Test.Rules.Exceptions.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Exceptions.xml TestResult.xml

exceptions_rules_sources_in = ../../AssemblyInfo.cs.in
exceptions_rules_generated_sources = $(exceptions_rules_sources_in:.in=)
exceptions_rules_sources = DontDestroyStackTrace.cs ISEHCatchBlock.cs \
	ISEHGuardedBlock.cs ISEHHandlerBlock.cs \
	SEHHandlerType.cs Impl/ExceptionBlockParser.cs Impl/ExecutionBlock.cs \
	Impl/ExecutionPath.cs Impl/ExecutionPathFactory.cs Impl/SEHCatchBlock.cs \
	Impl/SEHGuardedBlock.cs Impl/SEHHandlerBlock.cs Impl/SEHHandlerBlockCollection.cs

exceptions_rules_build_sources = $(addprefix $(srcdir)/, $(exceptions_rules_sources))
exceptions_rules_build_sources += $(exceptions_rules_generated_sources)

Gendarme.Rules.Exceptions.dll: $(exceptions_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(exceptions_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Exceptions.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	rm -f $(DESTDIR)$(pkglibdir)/`basename Gendarme.Rules.Exceptions.xml`;
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

exceptions_test_sources = TestPatterns.cs 
exceptions_test_build_sources = $(addprefix $(srcdir)/Test/, $(exceptions_test_sources))

Test.Rules.Exceptions.dll: $(exceptions_test_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Exceptions.dll -out:$@ $(exceptions_test_build_sources)

test: Gendarme.Rules.Exceptions.dll Test.Rules.Exceptions.dll

run-test: Gendarme.Rules.Exceptions.dll Test.Rules.Exceptions.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Exceptions.dll
