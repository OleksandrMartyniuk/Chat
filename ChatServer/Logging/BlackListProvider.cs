﻿using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public static class BlackListProvider
    {
        private static string Folder = "Authorization";
        private static string BlackList ="/BlackList";

        static BlackListProvider()
        {
            Directory.CreateDirectory(Folder);
        }

        public static void AppendRecord(string Username, TimeSpan duration)
        {
            File.AppendAllLines(Folder + BlackList, new string[] 
            {
                JsonConvert.SerializeObject(new BlackListRecord(DateTime.Now, Username, duration))
            });
        }

        public static bool RecordExists(string username)
        {
            LinkedList<BlackListRecord> list = GetBanList();
            foreach(BlackListRecord record in list)
            {
                if(record.Username == username)
                {
                    return true;
                }
            }
            return false;
        }

        public static void RemoveRecord(string Username)
        {
            LinkedList<BlackListRecord> records = GetBanList();
            foreach(BlackListRecord r in records)
            {
                if(r.Username == Username)
                {
                    records.Remove(r);
                    break;
                }
            }
        }

        public static TimeSpan GetDateTillBanDiscard(string username)
        {
            LinkedList<BlackListRecord> records = GetBanList();
            foreach(BlackListRecord rec in records)
            {
                if(username == rec.Username)
                {//////////////////////////////////////////////////////
                    DateTime final = rec.From.Add(rec.Duration);
                    TimeSpan timeLeft = DateTime.Now.Subtract(final);
                    return timeLeft;
                }/////////////////////////////////////////////////
            }
            return default(TimeSpan);
        }

        private static void SetBlackList(LinkedList<BlackListRecord> records)
        {
            File.Delete(Folder + BlackList);
            string[] lines = new string[records.Count];

            LinkedListNode<BlackListRecord> curr = records.First;
            for(int i=0; i< lines.Length; i++)
            {
                lines[i] = JsonConvert.SerializeObject(curr);
                curr = curr.Next;
            }

            File.WriteAllLines(Folder + BlackList, lines);
        }


        private static LinkedList<BlackListRecord> GetBanList()
        {
            if (!File.Exists(Folder + BlackList))
            {
                return new LinkedList<BlackListRecord>();  ///Правка //////////////////////////////////////////////
            }

            string[] list = File.ReadAllLines(Folder + BlackList);
            LinkedList<BlackListRecord> records = new LinkedList<BlackListRecord>();
            int length = list.Length;
            for (int i = 0; i < length; i++)
            {
                BlackListRecord rec = JsonConvert.DeserializeObject<BlackListRecord>(list[i]);
                records.AddLast(rec);
            }
            return records;
        }

        private class BlackListRecord
        {
            public BlackListRecord(DateTime from, string username, TimeSpan duration)
            {
                this.From = from;
                this.Username = username;
                this.Duration = duration;
            }
            public DateTime From { get; set; }
            public string Username { get; set; }

            public TimeSpan Duration { get; set; } 
        }
    }
}
