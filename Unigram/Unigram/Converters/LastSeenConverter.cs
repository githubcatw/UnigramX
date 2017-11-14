﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Api.TL;
using Unigram.Common;
using Windows.UI.Xaml.Data;

namespace Unigram.Converters
{
    public class LastSeenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var user = value as TLUser;
            if (user != null)
            {
                return GetLabel(user, parameter == null);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static int GetIndex(TLUser user)
        {
            if (user.IsBot)
            {
                // Last
                return user.IsBotChatHistory ? 1 : 0;
            }

            if (user.HasStatus && user.Status != null)
            {
                switch (user.Status)
                {
                    case TLUserStatusOffline offline:
                        return offline.WasOnline;
                    case TLUserStatusOnline online:
                        return int.MaxValue;
                    case TLUserStatusRecently recently:
                        // recently
                        // Before within a week
                        return 5;
                    case TLUserStatusLastWeek lastWeek:
                        // within a week
                        // Before within a month
                        return 4;
                    case TLUserStatusLastMonth lastMonth:
                        // within a month
                        // Before long time ago
                        return 3;
                    case TLUserStatusEmpty empty:
                    default:
                        // long time ago
                        // Before bots
                        return 2;
                }
            }

            // long time ago
            // Before bots
            return 2;
        }

        public static string GetLabel(TLUser user, bool details)
        {
            if (user.Id == 777000)
            {
                return Strings.Resources.ServiceNotificationsLabel;
            }
            else if (user.IsBot)
            {
                if (details)
                {
                    return Strings.Resources.GenericBotStatus;
                }

                return user.IsBotChatHistory ? Strings.Resources.BotStatusReadsGroupHistory : Strings.Resources.BotStatusDoesNotReadGroupHistory;
            }
            else if (user.IsSelf && details)
            {
                return Strings.Resources.CloudStorageChatStatus;
            }

            if (user.HasStatus && user.Status != null)
            {
                switch (user.Status)
                {
                    case TLUserStatusOffline offline:
                        var now = DateTime.Now;
                        var seen = TLUtils.ToDateTime(offline.WasOnline);
                        if (details)
                        {
                            return string.Format(Strings.Resources.LastSeen2, ((now.Date == seen.Date) ? Strings.Resources.Today : (((now.Date - seen.Date) == new TimeSpan(1, 0, 0, 0)) ? Strings.Resources.Yesterday : BindConvert.Current.ShortDate.Format(seen))), BindConvert.Current.ShortTime.Format(seen));
                        }

                        break;
                    case TLUserStatusOnline online:
                        return Strings.Resources.UserOnline;
                    case TLUserStatusRecently recently:
                        return string.Format(Strings.Resources.LastSeen1, Strings.Resources.Recently);
                    case TLUserStatusLastWeek lastWeek:
                        return string.Format(Strings.Resources.LastSeen1, Strings.Resources.WithinAWeek);
                    case TLUserStatusLastMonth lastMonth:
                        return string.Format(Strings.Resources.LastSeen1, Strings.Resources.WithinAMonth);
                    case TLUserStatusEmpty empty:
                    default:
                        return string.Format(Strings.Resources.LastSeen1, Strings.Resources.ALongTimeAgo);
                }
            }

            // Debugger.Break();
            return string.Format(Strings.Resources.LastSeen1, Strings.Resources.ALongTimeAgo);
        }
    }
}
