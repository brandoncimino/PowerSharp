using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PowerSharp
{
    public class Smushy<T> where T : new()
    {
        public List<T> Stuff = new List<T>();

        public T Child
        {
            get
            {
                return Stuff.FirstOrDefault();
            }

            set
            {
                if (Stuff.Count < 1)
                {
                    Stuff.Prepend(value);
                }
                else
                {
                    Stuff[0] = value;
                }
            }
        }

        public T Parent
        {
            get
            {
                return Stuff.LastOrDefault();
            }

            set
            {
                if (Stuff.Count < 2)
                {
                    Stuff.Append(value);
                }
                else
                {
                    Stuff[Stuff.Count - 1] = value;
                }
            }
        }

        public bool CombineCollections = true;

        public T Smushed => Smusher.Smush(Stuff, CombineCollections);

        public Type Type => typeof(T);

        #region Constructors
        public Smushy(IEnumerable<T> stuff)
        {
            this.Stuff = stuff.ToList();
        }

        public Smushy(T child, T parent) : this(new List<T> { child, parent }) { }

        public Smushy(params T[] stuff) : this((IEnumerable<T>)stuff) { }
        #endregion

        #region Factories
        public static Smushy<T> Of(IEnumerable<T> stuff) => new Smushy<T>(stuff);
        public static Smushy<T> Of(T child, T parent) => new Smushy<T>(child, parent);
        public static Smushy<T> Of(params T[] stuff) => new Smushy<T>((IEnumerable<T>)stuff);
        #endregion
    }
}