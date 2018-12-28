 public static class ObjectDumyDataExtensions
    {
        static Random _rnd = new Random();

        static int _deep = 0;

        public static T GenerateData<T>() where T : class, new()
        {
            _deep = 0;
            var instanceType = typeof(T);
            var instance = new T();

            var instanceProperties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            foreach (var pi in instanceProperties)
            {
                try
                {
                    if (IsIgnore(pi)) continue;

                    var val = BuildDumyDataForProp(pi.PropertyType);
                    if (pi.GetSetMethod() != null)
                    {
                        pi.SetValue(instance, val);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return instance;
        }

        static object BuildInstance(Type instanceType)
        {
            var instance = Activator.CreateInstance(instanceType);

            var instanceProperties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            foreach (var pi in instanceProperties)
            {
                try
                {
                    if (IsIgnore(pi)) continue;

                    var val = BuildDumyDataForProp(pi.PropertyType);
                    if (pi.GetSetMethod() != null)
                    {
                        pi.SetValue(instance, val);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return instance;
        }

        static bool IsIgnore(PropertyInfo pi)
        {
            var piType = pi.PropertyType;

            object[] attrs = pi.GetCustomAttributes(true);
            foreach (object attr in attrs)
            {
                Newtonsoft.Json.JsonIgnoreAttribute authAttr = attr as Newtonsoft.Json.JsonIgnoreAttribute;
                if (authAttr != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static object BuildDumyDataForProp(Type piType)
        {
            if (piType.IsAbstract || piType.IsInterface)
            {
                return null;
            }
            var enumNullType = Nullable.GetUnderlyingType(piType);

            if (piType.IsEnum || (enumNullType != null && enumNullType.IsEnum))
            {
                foreach (var e in Enum.GetValues(piType))
                {
                    return e;
                }
            }

            if (piType == typeof(string) || piType == typeof(String))
            {
                return "String_" + _rnd.Next();
            }
            if (piType == typeof(char) || piType == typeof(Char))
            {
                return (char)_rnd.Next(1, 128);
            }
            if (piType == typeof(Int16) || piType == typeof(Int16?))
            {
                return (Int16)_rnd.Next(1, 128);
            }
            if (piType == typeof(Int32) || piType == typeof(Int32?))
            {
                return (Int32)_rnd.Next(1, 128);
            }
            if (piType == typeof(Int64) || piType == typeof(Int64?))
            {
                return (Int64)_rnd.Next(1, 128);
            }
            if (piType == typeof(DateTime) || piType == typeof(DateTime?))
            {
                return DateTime.Now;
            }
            if (piType == typeof(float) || piType == typeof(float?))
            {
                return (float)_rnd.Next(1, 128);
            }
            if (piType == typeof(double) || piType == typeof(double?))
            {
                return _rnd.NextDouble();
            }
            if (piType == typeof(decimal) || piType == typeof(decimal?))
            {
                return (decimal)_rnd.Next(1, 128);
            }

            if (piType == typeof(Guid) || piType == typeof(Guid?))
            {
                return Guid.NewGuid();
            }

            if (piType == typeof(bool) || piType == typeof(bool?))
            {
                return _rnd.Next(0, 1) == 0;
            }

            if (piType.IsArray)
            {
                ArrayList items = new ArrayList();
                var elementType = piType.GetElementType();
                var item = BuildDumyDataForProp(elementType);
                items.Add(item);

                return items.ToArray(elementType);
            }

            if (piType.IsClass)
            {
                //  if (_deep > 3000) return null;
                _deep++;
                var proInstance = BuildInstance(piType);

                return proInstance;
            }

            return null;
        }
    }
