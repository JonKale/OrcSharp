﻿/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace OrcSharp.Types
{
    using System;
    using System.Numerics;

    sealed public class HiveDecimal : IEquatable<HiveDecimal>, IComparable<HiveDecimal>, IComparable
    {
        public const int MAX_PRECISION = 38;
        public const int MAX_SCALE = 38;
        public const int SYSTEM_DEFAULT_PRECISION = 38;
        public const int SYSTEM_DEFAULT_SCALE = 18;
        public const int USER_DEFAULT_PRECISION = 10;
        public const int USER_DEFAULT_SCALE = 0;

        public static readonly HiveDecimal Zero = new HiveDecimal(BigInteger.Zero, 0);
        public static readonly HiveDecimal One = HiveDecimal.create(BigInteger.One, 0);
        private static readonly BigInteger Ten = new BigInteger(10);

        BigInteger mantissa;
        int _scale;

        HiveDecimal(BigInteger mantissa, int scale = 0)
        {
            this.mantissa = mantissa;
            this._scale = scale;
        }

        public HiveDecimal(int value, int scale = 0)
            : this(new BigInteger(value), scale)
        {
        }

        public HiveDecimal(long value, int scale = 0)
            : this(new BigInteger(value), scale)
        {
        }

        public static HiveDecimal create(BigInteger mantissa, int scale = 0)
        {
            return new HiveDecimal(mantissa, scale);
        }

        public static HiveDecimal create(int mantissa, int scale = 0)
        {
            return new HiveDecimal(new BigInteger(mantissa), scale);
        }

        public static HiveDecimal Parse(string text)
        {
            // TODO: Yuck
            int position = text.IndexOf('.');
            int scale = 0;
            if (position >= 0)
            {
                text = text.Remove(position, 1);
                scale = text.Length - position;
            }

            return new HiveDecimal(BigInteger.Parse(text), scale);
        }

        public static HiveDecimal operator +(HiveDecimal left, HiveDecimal right)
        {
            Normalize(ref left, ref right);
            return new HiveDecimal(left.mantissa + right.mantissa, left._scale);
        }

        public int CompareTo(HiveDecimal value)
        {
            HiveDecimal me = this;
            Normalize(ref me, ref value);
            return me.mantissa.CompareTo(value.mantissa);
        }

        public override string ToString()
        {
            string result = this.mantissa.ToString();
            if (_scale > 0)
            {
                int shift = result.Length - _scale;
                if (shift < 0)
                {
                    result = result.PadLeft(-shift, '0');
                    shift = 0;
                }
                result = result.Insert(shift, ".");
            }
            else if (_scale < 0)
            {
                result = result + new string('0', -_scale);
            }
            return result;
        }

        public static HiveDecimal enforcePrecisionScale(HiveDecimal hiveDec, int precision, int scale)
        {
            // TODO:
            return hiveDec;
        }

        public int scale()
        {
            return _scale;
        }

        static void Normalize(ref HiveDecimal x, ref HiveDecimal y)
        {
            if (x._scale > y._scale)
            {
                y = y.UpdateScale(x._scale);
            }
            else if (y._scale > x._scale)
            {
                x = x.UpdateScale(y._scale);
            }
        }

        HiveDecimal UpdateScale(int newScale)
        {
            return new HiveDecimal(mantissa * BigInteger.Pow(Ten, newScale - _scale), newScale);
        }

        public long longValue()
        {
            if (_scale < 0)
            {
                return (long)(mantissa * BigInteger.Pow(Ten, -_scale));
            }
            else if (_scale > 0)
            {
                return (long)(mantissa / BigInteger.Pow(Ten, _scale));
            }
            else
            {
            return (long)this.mantissa;
            }
        }

        internal BigInteger unscaledValue()
        {
            return mantissa;
        }

        internal double doubleValue()
        {
            // TODO:
            return (double)mantissa * Math.Pow(10, -_scale);
        }

        public override bool Equals(object obj)
        {
            HiveDecimal other = obj as HiveDecimal;
            return other == null ? false : Equals(other);
        }

        public bool Equals(HiveDecimal other)
        {
            HiveDecimal x = this;
            Normalize(ref x, ref other);
            return x._scale == other._scale && x.mantissa == other.mantissa;
        }

        public int CompareTo(object obj)
        {
            return CompareTo((HiveDecimal)obj);
        }

        public override int GetHashCode()
        {
            return mantissa.GetHashCode() ^ _scale;
        }
    }
}
