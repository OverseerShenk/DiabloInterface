﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiabloInterface
{
    class VersionChecker
    {

        static string releasesUrl = "https://github.com/Zutatensuppe/DiabloInterface/releases";
        static string releasesLatestUrl = "https://github.com/Zutatensuppe/DiabloInterface/releases/latest";

        public static void CheckForUpdate( bool userTriggered )
        {
            string updateUrl = getUpdateUrl();
            if (updateUrl != null)
            {
                if (MessageBox.Show("A new version of DiabloInterface is available. Go to download page now?", "New version available",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(updateUrl);
                }
            } else if ( userTriggered )
            {
                if (MessageBox.Show("No new version is available, but there might be a pre-release. Go to releases overview now?", "No new version available",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(releasesUrl);
                }
            }
        }
        

        public static string getUpdateUrl()
        {
            Match verMatch = Regex.Match(Application.ProductVersion, @"^(\d+)\.(\d+)\.(\d+)(?:\.PR\.(\d+))?$");
            if (!verMatch.Success)
            {
                return "";
            }
            int major = Convert.ToInt32(verMatch.Groups[1].Value);
            int minor = Convert.ToInt32(verMatch.Groups[2].Value);
            int patch = Convert.ToInt32(verMatch.Groups[3].Value);
            int pre = 0;
            if (verMatch.Groups.Count >= 5)
            {
                pre = Convert.ToInt32(verMatch.Groups[4].Value);
            }

            HttpWebRequest r = (HttpWebRequest)WebRequest.Create(releasesLatestUrl);
            r.Method = "HEAD";
            r.AllowAutoRedirect = false;
            
            string location;
            using (var response = r.GetResponse() as HttpWebResponse)
            {
                location = response.GetResponseHeader("Location");
            }

            Match tagMatch = Regex.Match(location, @"/releases/tag/v(\d+)\.(\d+)\.(\d+)$");

            if (!tagMatch.Success)
            {
                return null;
            }

            int majorNew = Convert.ToInt32(tagMatch.Groups[1].Value);
            int minorNew = Convert.ToInt32(tagMatch.Groups[2].Value);
            int patchNew = Convert.ToInt32(tagMatch.Groups[3].Value);


            // version compare.

            if (majorNew > major)
            {
                return location;
            }
            else if (majorNew < major)
            {
                return null;
            }
            else if (minorNew > minor)
            {
                return location;
            }
            else if (minorNew < minor)
            {
                return null;
            }
            else if (patchNew > patch)
            {
                return location;
            }
            else if (patchNew < patch)
            {
                return null;
            } else if ( pre > 0 )
            {
                return location;
            } else
            {
                return null;
            }
            
        }
    }
}
