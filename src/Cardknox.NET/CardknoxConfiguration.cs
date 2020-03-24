using System;
using System.Collections.Generic;
using System.Text;

namespace CardknoxApi.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static class CardknoxConfiguration
    {
        /// <summary>
        /// Enable automatic logging. <see cref="LogLocation"/> must be set to a valid path to work.
        /// </summary>
        public static bool LoggingEnabled { get; set; } = false;

        /// <summary>
        /// <para>Can be app-specific by prepending the ~ character or it can be absolute by omitting it</para>
        /// <para>The sensitive card information (xCardNum, xCVV, xExp, xMagStripe, etc) will not be included for PCI compliance reasons.</para>
        /// <para>Examples:</para>
        /// <list type="bullet">
        /// <item>~/App_Data/Log/cardknox_{0:MMddyyyy}.log</item>
        /// <item>/var/log/cardknox/{0:MMddyyyy}.log</item>
        /// <item>C:\Temp\Cardknox\{0:MMddyyyy}.log</item>
        /// </list>
        /// </summary>
        public static string LogLocation { get; set; } = "";
    }
}
