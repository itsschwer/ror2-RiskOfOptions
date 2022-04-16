﻿using System;
using System.Text;
using UnityEngine;

namespace RiskOfOptions.Components.AssetResolution.Data
{
    internal class UnityByteBufReader : UnityByteBuf
    {
        private uint _pos;
        
        internal UnityByteBufReader(byte[] bytes)
        {
            byteBuffer = bytes;
            _pos = 0;
        }
        
        internal byte[] ReadByteArray()
        {
            var length = ReadUInt();

            return ReadBytes(length);
        }

        internal string ReadString()
        {
            var stringBytes = ReadByteArray();

            return Encoding.UTF8.GetString(stringBytes);
        }

        internal uint ReadUInt()
        {
            var bytes = ReadBytes(4);

            return BitConverter.ToUInt32(bytes, 0);
        }

        internal int ReadInt()
        {
            var bytes = ReadBytes(4);

            return BitConverter.ToInt32(bytes, 0);
        }

        internal float ReadFloat()
        {
            var bytes = ReadBytes(4);

            return BitConverter.ToSingle(bytes, 0);
        }

        internal double ReadDouble()
        {
            var bytes = ReadBytes(8);

            return BitConverter.ToDouble(bytes, 0);
        }

        internal Rect ReadRect()
        {
            var floats = ReadFloats(4);

            return new Rect(floats[0], floats[1], floats[2], floats[3]);
        }

        internal Vector2 ReadVector2()
        {
            var floats = ReadFloats(2);
            
            return new Vector2(floats[0], floats[1]);
        }

        internal Vector3 ReadVector3()
        {
            var floats = ReadFloats(3);

            return new Vector3(floats[0], floats[1], floats[2]);
        }

        internal Vector4 ReadVector4()
        {
            var floats = ReadFloats(4);

            return new Vector4(floats[0], floats[1], floats[2], floats[3]);
        }

        internal T ReadEnum<T>() where T : Enum
        {
            var value = ReadInt();

            return (T)Enum.ToObject(typeof(T), value);
        }

        internal T ReadComponentReference<T>(Transform root) where T : Component
        {
            var path = ReadString();

            return root.transform.Find(path).gameObject.GetComponent<T>();
        }

        private float[] ReadFloats(int length)
        {
            float[] floats = new float[length];

            for (int i = 0; i < length; i++)
                floats[i] = ReadFloat();

            return floats;
        }
        

        private byte ReadByte()
        {
            byte data = byteBuffer[_pos];
            _pos++;

            return data;
        }

        private byte[] ReadBytes(uint length)
        {
            byte[] buffer = new byte[length];
            
            Array.Copy(byteBuffer, _pos, buffer, 0, length);

            _pos += length;

            return buffer;
        }
    }
}