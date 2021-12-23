using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

public class Compressing : MonoBehaviour
{
    public class PriorityQueue<T> where T : IComparable
    {
        protected List<T> LstHeap = new List<T>();
        public virtual int Count
        {
            get { return LstHeap.Count; }
        }

        public virtual void Add(T val)
        {
            LstHeap.Add(val);
            SetAt(LstHeap.Count - 1, val);
            UpHeap(LstHeap.Count - 1);
        }

        public virtual T Peek()
        {
            if (LstHeap.Count == 0)
            {
                throw new IndexOutOfRangeException("Error: Trying to peek at an empty priority queue");
            }
            return LstHeap[0];
        }

        public virtual T Pop()
        {
            if (LstHeap.Count == 0)
            {
                throw new IndexOutOfRangeException("Error: Trying to popping an empty priority queue");
            }

            T valRet = LstHeap[0];

            SetAt(0, LstHeap[LstHeap.Count - 1]);
            LstHeap.RemoveAt(LstHeap.Count - 1);
            DownHeap(0);
            return valRet;
        }

        protected virtual void SetAt(int i, T val)
        {
            LstHeap[i] = val;
        }

        protected bool RightSonExists(int i)
        { return RChildIndex(i) < LstHeap.Count; }

        protected bool LeftSonExists(int i)
        { return LChildIndex(i) < LstHeap.Count; }

        protected int ParentIndex(int i)
        { return (i - 1) / 2; }

        protected int LChildIndex(int i)
        { return 2 * i + 1; }

        protected int RChildIndex(int i)
        { return 2 * (i + 1); }

        protected T ArrayVal(int i)
        { return LstHeap[i]; }

        protected T Parent(int i)
        { return LstHeap[ParentIndex(i)]; }

        protected T Left(int i)
        { return LstHeap[LChildIndex(i)]; }

        protected T Right(int i)
        { return LstHeap[RChildIndex(i)]; }

        protected void Swap(int i, int j)
        {
            T valHold = ArrayVal(i);
            SetAt(i, LstHeap[j]);
            SetAt(j, valHold);
        }

        protected void UpHeap(int i)
        {
            while (i > 0 && ArrayVal(i).CompareTo(Parent(i)) > 0)
            {
                Swap(i, ParentIndex(i));
                i = ParentIndex(i);
            }
        }

        protected void DownHeap(int i)
        {
            while (i >= 0)
            {
                int iContinue = -1;

                if (RightSonExists(i) && Right(i).CompareTo(ArrayVal(i)) > 0)
                {
                    iContinue = Left(i).CompareTo(Right(i)) < 0 ? RChildIndex(i) : LChildIndex(i);
                }
                else if (LeftSonExists(i) && Left(i).CompareTo(ArrayVal(i)) > 0)
                {
                    iContinue = LChildIndex(i);
                }

                if (iContinue >= 0 && iContinue < LstHeap.Count)
                {
                    Swap(i, iContinue);
                }

                i = iContinue;
            }
        }
    }
    internal class HuffNode<T> : IComparable
    {
        internal HuffNode(double probability, T value)
        {
            Probability = probability;
            LeftSon = RightSon = Parent = null;
            Value = value;
            Is_a_Leaf = true;
        }

        internal HuffNode(HuffNode<T> leftSon, HuffNode<T> rightSon)
        {
            RightSon = rightSon;
            LeftSon = leftSon;
            Probability = leftSon.Probability + rightSon.Probability;
            rightSon.IsZero = false;
            leftSon.IsZero = true;
            leftSon.Parent = rightSon.Parent = this;
            Is_a_Leaf = false;
        }

        internal HuffNode<T> LeftSon { get; set; }
        internal HuffNode<T> RightSon { get; set; }
        internal HuffNode<T> Parent { get; set; }
        internal T Value { get; set; }
        internal bool Is_a_Leaf { get; set; }
        internal bool IsZero { get; set; }
        internal int Bit
        {
            get { return IsZero ? 0 : 1; }
        }

        internal bool IsRoot
        {
            get { return Parent == null; }
        }

        internal double Probability { get; set; }

        public int CompareTo(object obj)
        {
            return -Probability.CompareTo(((HuffNode<T>)obj).Probability);
        }
    }

    public class Huffman<T> where T : IComparable
    {
        private readonly Dictionary<T, HuffNode<T>> _leafDictionary = new Dictionary<T, HuffNode<T>>();
        private readonly HuffNode<T> _root;

        public Huffman(IEnumerable<T> values)
        {
            var counts = new Dictionary<T, int>();
            var priorityQueue = new PriorityQueue<HuffNode<T>>();
            int valueCount = 0;

            foreach (T value in values)
            {
                if (!counts.ContainsKey(value))
                {
                    counts[value] = 0;
                }
                counts[value]++;
                valueCount++;
            }

            foreach (T value in counts.Keys)
            {
                var node = new HuffNode<T>((double)counts[value] / valueCount, value);
                priorityQueue.Add(node);
                _leafDictionary[value] = node;
            }

            while (priorityQueue.Count > 1)
            {
                HuffNode<T> leftSon = priorityQueue.Pop();
                HuffNode<T> rightSon = priorityQueue.Pop();
                var parent = new HuffNode<T>(leftSon, rightSon);
                priorityQueue.Add(parent);
            }

            _root = priorityQueue.Pop();
            _root.IsZero = false;
        }

        public List<int> Encode(T value)
        {
            var returnValue = new List<int>();
            Encode(value, returnValue);
            return returnValue;
        }

        public void Encode(T value, List<int> encoding)
        {
            if (!_leafDictionary.ContainsKey(value))
            {
                throw new ArgumentException("wrong value inside the Encode");
            }
            HuffNode<T> nodeCur = _leafDictionary[value];
            var reverseEncoding = new List<int>();
            while (!nodeCur.IsRoot)
            {
                reverseEncoding.Add(nodeCur.Bit);
                nodeCur = nodeCur.Parent;
            }
            reverseEncoding.Reverse();
            encoding.AddRange(reverseEncoding);
        }

        public List<int> Encode(IEnumerable<T> values)
        {
            var returnValue = new List<int>();
            foreach (T value in values)
            {
                Encode(value, returnValue);
            }
            return returnValue;
        }

        public T Decode(List<int> bitString, ref int position)
        {
            HuffNode<T> nodeCur = _root;
            while (!nodeCur.Is_a_Leaf)
            {
                if (position > bitString.Count)
                {
                    throw new ArgumentException("wrong bitstring inside Decode");
                }
                nodeCur = bitString[position++] == 0 ? nodeCur.LeftSon : nodeCur.RightSon;
            }
            return nodeCur.Value;
        }

        public List<T> Decode(List<int> bitString)
        {
            int position = 0;
            var returnValue = new List<T>();

            while (position != bitString.Count)
            {
                returnValue.Add(Decode(bitString, ref position));
            }
            return returnValue;
        }
    }

    public void Compress()
    {
        string text = GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text; 
        byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text =Convert.ToBase64String(gZipBuffer);
        
    }


    public void DeCompress()
    {
        string compressedText = GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text;
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
        using (var memoryStream = new MemoryStream())
        {
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

            var buffer = new byte[dataLength];

            memoryStream.Position = 0;
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gZipStream.Read(buffer, 0, buffer.Length);
            }

            GameObject.FindGameObjectWithTag("mainText").GetComponent<UnityEngine.UI.InputField>().text = Encoding.UTF8.GetString(buffer);
        }
    }


    // Update is called once per frame

    void Update()
    {
        if (PlayerPrefs.GetInt("json") == 1)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;

        }
    }
}
