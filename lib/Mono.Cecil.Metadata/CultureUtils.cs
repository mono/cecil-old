/*
 * Copyright (c) 2004 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jb.evain@dotnetguru.org)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Metadata {

    using System;
    using System.Globalization;

    public sealed class CultureUtils {

        private static string[] cultures = new string[] {
            "ar-SA", "ar-IQ", "ar-EG", "ar-LY",
            "ar-DZ", "ar-MA", "ar-TN", "ar-OM",
            "ar-YE", "ar-SY", "ar-JO", "ar-LB",
            "ar-KW", "ar-AE", "ar-BH", "ar-QA",
            "bg-BG", "ca-ES", "zh-TW", "zh-CN",
            "zh-HK", "zh-SG", "zh-MO", "cs-CZ",
            "da-DK", "de-DE", "de-CH", "de-AT",
            "de-LU", "de-LI", "el-GR", "en-US",
            "en-GB", "en-AU", "en-CA", "en-NZ",
            "en-IE", "en-ZA", "en-JM", "en-CB",
            "en-BZ", "en-TT", "en-ZW", "en-PH",
            "es-ES-Ts", "es-MX", "es-ES-Is", "es-GT",
            "es-CR", "es-PA", "es-DO", "es-VE",
            "es-CO", "es-PE", "es-AR", "es-EC",
            "ec-CL", "es-UY", "es-PY", "es-BO",
            "es-SV", "es-HN", "es-NI", "es-PR",
            "Fi-FI", "fr-FR", "fr-BE", "fr-CA",
            "Fr-CH", "fr-LU", "fr-MC", "he-IL",
            "hu-HU", "is-IS", "it-IT", "it-CH",
            "Ja-JP", "ko-KR", "nl-NL", "nl-BE",
            "nb-NO", "nn-NO", "pl-PL", "pt-BR",
            "pt-PT", "ro-RO", "ru-RU", "hr-HR",
            "Lt-sr-SP", "Cy-sr-SP", "sk-SK  sq-AL",
            "sv-SE", "sv-FI", "th-TH", "tr-TR",
            "ur-PK", "id-ID", "uk-UA", "be-BY",
            "sl-SI", "et-EE", "lv-LV", "lt-LT",
            "fa-IR", "vi-VN", "hy-AM", "Lt-az-AZ",
            "Cy-az-AZ", "eu-ES", "mk-MK", "af-ZA",
            "ka-GE", "fo-FO", "hi-IN", "ms-MY",
            "ms-BN", "kk-KZ", "ky-KZ", "sw-KE",
            "Lt-uz-UZ", "Cy-uz-UZ", "tt-TA", "pa-IN",
            "gu-IN", "ta-IN", "te-IN", "kn-IN",
            "mr-IN", "sa-IN", "mn-MN", "gl-ES",
            "Kok-IN", "syr-SY", "div-MV", "neutral",
            string.Empty
        };

        public static bool IsValid(string culture) {
            if (culture == null) {
                throw new ArgumentException("culture");
            }
            foreach (string cult in cultures) {
                if (culture.ToLower() == cult.ToLower()) {
                    return true;
                }
            }
            return false;
        }

        public static CultureInfo GetCultureInfo(string culture) {
            if (IsValid(culture)) {
                if (culture.Length == 0 || culture == "neutral") {
                    return CultureInfo.InvariantCulture;
                } else {
                    foreach (CultureInfo ci in CultureInfo.GetCultures(
                        CultureTypes.AllCultures)) {

                        if (ci.Name == culture) {
                            return ci;
                        }
                    }
                }
            }
            return CultureInfo.InvariantCulture;
        }
    }
}
