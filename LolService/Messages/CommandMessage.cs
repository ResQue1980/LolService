﻿using System.Collections.Generic;
using FluorineFx.AMF3;

namespace LolService.Messages
{
    public class CommandMessage : AsyncMessage
    {
        public const byte OPERATION_FLAG = 1;
        public const uint SUBSCRIBE_OPERATION = 0;
        public const uint CLIENT_SYNC_OPERATION = 4;
        public const uint SUBSCRIPTION_INVALIDATE_OPERATION = 10;
        public const uint TRIGGER_CONNECT_OPERATION = 13;
        public const uint CLIENT_PING_OPERATION = 5;
        public const uint UNSUBSCRIBE_OPERATION = 1;
        public const uint POLL_OPERATION = 2;
        public const uint MULTI_SUBSCRIBE_OPERATION = 11;
        public const uint LOGIN_OPERATION = 8;
        public const uint CLUSTER_REQUEST_OPERATION = 7;
        public const uint LOGOUT_OPERATION = 9;
        public const uint UNKNOWN_OPERATION = 10000;
        public const uint DISCONNECT_OPERATION = 12;

        static Dictionary<uint, string> operationtexts = new Dictionary<uint, string>();
        static CommandMessage()
        {
            operationtexts[SUBSCRIBE_OPERATION] = "subscribe";
            operationtexts[UNSUBSCRIBE_OPERATION] = "unsubscribe";
            operationtexts[POLL_OPERATION] = "poll";
            operationtexts[CLIENT_SYNC_OPERATION] = "client sync";
            operationtexts[CLIENT_PING_OPERATION] = "client ping";
            operationtexts[CLUSTER_REQUEST_OPERATION] = "cluster request";
            operationtexts[LOGIN_OPERATION] = "login";
            operationtexts[LOGOUT_OPERATION] = "logout";
            operationtexts[SUBSCRIPTION_INVALIDATE_OPERATION] = "subscription invalidate";
            operationtexts[MULTI_SUBSCRIBE_OPERATION] = "multi-subscribe";
            operationtexts[DISCONNECT_OPERATION] = "disconnect";
            operationtexts[TRIGGER_CONNECT_OPERATION] = "trigger connect";
            operationtexts[UNKNOWN_OPERATION] = "unknown";
        }

        public uint Operation { get; set; }
        public string OperationString { get; set; }

        protected void SetOperation(uint num)
        {
            Operation = num;

            string str;
            OperationString = operationtexts.TryGetValue(num, out str) ? str : num.ToString();
        }

        public override void ReadExternal(IDataInput input)
        {
            base.ReadExternal(input);
            var flags = ReadFlags(input);
            for (int i = 0; i < flags.Count; i++)
            {
                int bits = 0;
                if (i == 0)
                {
                    if ((flags[i] & OPERATION_FLAG) != 0)
                    {
                        SetOperation((uint)input.ReadObject());
                    }
                    bits = 1;
                }
                ReadRemaining(input, flags[i], bits);
            }
        }

        public override void WriteExternal(IDataOutput output)
        {
            base.WriteExternal(output);

            int flags = 0;
            if (Operation != 0)
                flags |= OPERATION_FLAG;

            output.WriteByte((byte)flags);

            if (Operation != 0)
                output.WriteObject(Operation);
        }
    }
}
