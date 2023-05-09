using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordCodec;

namespace PasswordManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Expression option = Expression.unknown;
            string password = null;
            if (!(ReadArguments.HasArguments(args)))
                Environment.Exit(1);
            foreach (DictionaryEntry de in ReadArguments.GetArguments(args))
            {
                if (de.Key.ToString().Equals(Enum.GetName(typeof(Options),Options.option)))
                {
                    option = (Expression)Enum.Parse(typeof(Expression), de.Value.ToString());
                }
                if (de.Key.ToString().Equals(Enum.GetName(typeof(Options),Options.password)))
                {
                    password = de.Value.ToString();
                }
            }
            switch (option)
            {
                case Expression.encode:
                    try
                    {
                        Console.WriteLine(Codec.EncodeText(password));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case Expression.encrypt:
                    try
                    {
                        Console.WriteLine(Codec.EncryptText(password));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case Expression.decrypt:
                    try
                    {
                        Console.WriteLine(Codec.DecryptText(password));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
    internal class ReadArguments
    {
        public static bool HasArguments(string[] args)
        {
            if (GetArguments(args) == null)
            {
                Help.Message();
                return false;
            }
            return true;
        }
        public static Hashtable GetArguments(string[] args)
        {
            Hashtable result = new Hashtable();
            int options = 0;
            if (args.Length != 4)
            {
                return null;
            }
            for (int i = 0; i < args.Length; i += 2)
            {
                if (!(args[i].StartsWith("--")))
                {
                    return null;
                }
                foreach (string item in Enum.GetNames(typeof(Options)))
                {
                    if ((args[i].ToLower() == ("--" + item)))
                    {
                        options++;
                    }
                }
            }
            if (options != Enum.GetNames(typeof(Options)).Length)
            {
                return null;
            }
            for (int i = 0; i < args.Length; i += 2)
            {
                foreach (string item in Enum.GetNames(typeof(Options)))
                {
                    if ((args[i].ToLower() == ("--" + item)))
                    {
                        if ((item == Options.option.ToString()) && (!(Enum.GetNames(typeof(Expression)).Contains(args[i + 1]))))
                                return null;
                        result.Add(item, args[i + 1]);
                        continue;
                    }
                }
            }
            return result;
        }
    }
    internal class Help
    {
        internal static void Message()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    to encode MasterKey:\n         PasswordManager --option encode --password MASTER_KEY_PASSWORD");
            Console.WriteLine("    to encrypt a password:\n         PasswordManager --option encrypt --password YOUR_CLEAR_TEXT_PASSWORD");
            Console.WriteLine("    to decrypt a password:\n         PasswordManager --option decrypt --password YOUR_ENCRYPTED_PASSWORD");
        }
    }
    enum Options
    {
        option,
        password
    }
    enum Expression
    {
        encode,
        encrypt,
        decrypt,
        unknown
    }
}
