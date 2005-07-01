/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Metadata {

    using System;
    using System.Collections;
    using System.Globalization;

    internal sealed class CultureUtils {

        private static IDictionary m_cultures;

        private CultureUtils ()
        {
        }

        private static void LoadCultures ()
        {
            if (m_cultures != null)
                return;

            CultureInfo [] cultures = CultureInfo.GetCultures (CultureTypes.AllCultures);
            m_cultures = new Hashtable (cultures.Length);

            foreach (CultureInfo ci in cultures)
                m_cultures.Add (ci.Name, ci);

            m_cultures.Add ("neutral", CultureInfo.InvariantCulture);
        }

        public static bool IsValid (string culture)
        {
            if (culture == null)
                throw new ArgumentNullException ("culture");

            LoadCultures ();

            return m_cultures.Contains (culture);
        }

        public static CultureInfo GetCultureInfo (string culture)
        {
            if (IsValid (culture))
                return m_cultures [culture] as CultureInfo;

            return CultureInfo.InvariantCulture;
        }
    }
}
