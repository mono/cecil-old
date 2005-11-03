AC_INIT(README)
AC_CANONICAL_SYSTEM
AM_INIT_AUTOMAKE(gendarme, 0.0.1.0)
AM_MAINTAINER_MODE

AC_PROG_INSTALL

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
	AC_MSG_ERROR([You need to install pkg-config])
fi

dnl C# compiler
AC_PATH_PROG(CSC, csc, no)
AC_PATH_PROG(MCS, mcs, no)
AC_PATH_PROG(MONO, mono, no)

CS="C#"
if test "x$CSC" = "xno" -a "x$MCS" = "xno"  ; then
	dnl AC_MSG_ERROR([You need to install a C# compiler])
	AC_MSG_ERROR([No $CS compiler found])
fi

if test "x$MCS" = "xno" ; then
	MCS=$CSC
fi

if test "x$MONO" = "xno"; then
	AC_MSG_ERROR([No mono runtime found])
fi

AC_SUBST(MCS)

PKG_CHECK_MODULES(MONO_CECIL, mono-cecil)
AC_SUBST(MONO_CECIL_LIBS)

dnl find assembly paths for unstable installed assemblies that will be
dnl copied into gendarme installation directory for local usage with the app
UNSTABLE_INSTALL_ASSEMBLIES="mono-cecil"
INSTALLED_ASSEMBLIES="`$PKG_CONFIG --variable=Libraries $UNSTABLE_INSTALL_ASSEMBLIES`"
AC_SUBST(INSTALLED_ASSEMBLIES)

CECIL_PATH="`$PKG_CONFIG --variable=assemblies_dir mono-cecil`"
AC_SUBST(CECIL_PATH)

AC_OUTPUT([
Makefile
AssemblyInfo.cs
man/Makefile
man/gendarme.1
console/Makefile
framework/Makefile
rules/Makefile
rules/Gendarme.Rules.Security/Makefile
rules/Gendarme.Rules.Security/Gendarme.Rules.Security.xml
rules/Gendarme.Rules.Security/Test/Makefile
rules/Gendarme.Rules.Performance/Makefile
rules/Gendarme.Rules.Performance/Gendarme.Rules.Performance.xml
rules/Gendarme.Rules.Performance/Test/Makefile
])
