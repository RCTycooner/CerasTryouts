using Ceras;
using System;
using System.Globalization;
using System.Linq;

namespace CerasTryouts
{
    public static class CerasTryer
    {
        internal static void DoStuff()
        {
            try
            {
                var dt = new DateTime(2019, 10, 23, 13, 45, 40, 0, DateTimeKind.Utc);
                Foo foo = new Foo() { ADateTime = dt };
                Foo2 foo2 = new Foo2() { ADateTime = dt };

                TryDefaultConfig(dt, foo);
                WrappedProperty(dt, foo2);
                TryConfig3(dt, foo);
                TryConfig2(dt, foo);
                TryConfig1(dt, foo);

            }
            catch (Exception ex)
            {

            }
        }

        private static void TryDefaultConfig(DateTime dt, Foo foo)
        {
            try
            {
                var cfg = new SerializerConfig() { PreserveReferences = false };
                CerasSerializer serializer = new CerasSerializer(cfg);
                byte[] serializerBuffer = null;

                var length = serializer.Serialize<Foo>(foo, ref serializerBuffer);
                throw new Exception("We shouldn't be getting to this location, we expect Serialize() to throw a WarningException");

                //var serializedFoo = serializerBuffer.Take(length).ToArray();

                //Foo deserializedFoo = serializer.Deserialize<Foo>((byte[])serializedFoo);
                //if (deserializedFoo.ADateTime != dt)
                //    throw new Exception();
            }
            catch (Ceras.Exceptions.WarningException ex)
            {
                //as expected
            }
        }
        private static void WrappedProperty(DateTime dt, Foo2 foo)
        {
            try
            {
                var cfg = new SerializerConfig() { PreserveReferences = false };

                SerAndDeser(dt, foo, cfg);
            }
            catch (Ceras.Exceptions.WarningException ex)
            {
                throw ex; //Shouldn't be happening
            }
            catch (Exception ex)
            {
                throw ex; //Shouldn't be happening
            }
        }
        private static void TryConfig1(DateTime dt, Foo foo)
        {
            try
            {
                var cfg = new SerializerConfig() { PreserveReferences = false };
                cfg.ConfigType<DateTime>()
                    .ConfigMember(p => p.Kind).Include()
                    .ConfigMember(p => p.Ticks).Include();

                SerAndDeser(dt, foo, cfg);
            }
            catch (Ceras.Exceptions.WarningException ex)
            {
                throw ex; //Shouldn't be happening
            }
            catch (Exception ex)
            {
                throw ex; //Shouldn't be happening
            }
        }

        private static void TryConfig2(DateTime dt, Foo foo)
        {
            try
            {
                var cfg = new SerializerConfig() { PreserveReferences = false };
                cfg.ConfigType<DateTime>()
                    .CustomResolver = (c, t) => c.Advanced
                    .GetFormatterResolver<Ceras.Resolvers.DynamicObjectFormatterResolver>()
                    .GetFormatter(t);

                SerAndDeser(dt, foo, cfg);
            }
            catch (Ceras.Exceptions.WarningException ex)
            {
                throw ex; //Shouldn't be happening
            }
            catch (Exception ex)
            {
                throw ex; //Shouldn't be happening
            }
        }
        private static void TryConfig3(DateTime dt, Foo foo)
        {
            try
            {
                var cfg = new SerializerConfig() { PreserveReferences = false };
                cfg.ConfigType<DateTime>()
                    .ConfigField("dateData").Include();

                SerAndDeser(dt, foo, cfg);
            }
            catch (Ceras.Exceptions.WarningException ex)
            {
                throw ex; //Shouldn't be happening
            }
            catch (Exception ex)
            {
                throw ex; //Shouldn't be happening
            }
        }

        private static void SerAndDeser(DateTime dt, Foo foo, SerializerConfig cfg)
        {
            CerasSerializer serializer = new CerasSerializer(cfg);
            byte[] serializerBuffer = null;

            var length = serializer.Serialize<Foo>(foo, ref serializerBuffer);

            var serializedFoo = serializerBuffer.Take(length).ToArray();

            var deserializedFoo = serializer.Deserialize<Foo>((byte[])serializedFoo);
            if (deserializedFoo.ADateTime != dt)
                throw new Exception("Data is corrupt. Expected: " + dt.ToString() + ". Actual:" + deserializedFoo.ADateTime.ToString());
        }
        private static void SerAndDeser(DateTime dt, Foo2 foo, SerializerConfig cfg)
        {
            CerasSerializer serializer = new CerasSerializer(cfg);
            byte[] serializerBuffer = null;

            var length = serializer.Serialize<Foo2>(foo, ref serializerBuffer);

            var serializedFoo = serializerBuffer.Take(length).ToArray();

            var deserializedFoo = serializer.Deserialize<Foo2>((byte[])serializedFoo);
            if (deserializedFoo.ADateTime != dt)
                throw new Exception("Data is corrupt. Expected: " + dt.ToString() + ". Actual:" + deserializedFoo.ADateTime.ToString());
        }
    }


    [Ceras.MemberConfig(TargetMember.AllProperties)]
    public class Foo
    {
        public DateTime ADateTime { get; set; }

        public Foo()
        { }
    }

    [Ceras.MemberConfig(TargetMember.AllProperties)]
    public class Foo2
    {
        [Exclude()]
        public DateTime ADateTime { get; set; }
        public string ADateTimeAsString
        {
            get
            {
                return this.ADateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            }
            set
            {
                this.ADateTime = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss.ffff", CultureInfo.InvariantCulture);
            }
        }

        public Foo2()
        { }
    }
}