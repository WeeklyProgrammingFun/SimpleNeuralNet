﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet.Model
{
    public class DataPoint
    {
        public DataPoint(Vector input, Vector output)
        {
            this.input = input;
            this.output = output;
        }
        public Vector input;
        public Vector output;
    }

    public class DataSet
    {
        public List<DataPoint> TrainingSet { get;  } = new List<DataPoint>();
        public List<DataPoint> TestSet { get; } = new List<DataPoint>();

        public void Trim(int trainingSize, int testSize)
        {
            TrainingSet.RemoveRange(trainingSize, TrainingSet.Count - trainingSize);
            TestSet.RemoveRange(testSize, TestSet.Count - testSize);
        }
        class ByteBuffer
        {
            private readonly byte[] buffer;
            private int index;
            public ByteBuffer(string filename)
            {
                buffer = File.ReadAllBytes(filename);
            }

            public uint Read(int n = 1)
            {
                uint v = 0;
                while (n-- > 0)
                {
                    v *= 256;
                    v += buffer[index++];
                }

                return v;
            }
        }

        void Load(
            ByteBuffer dataBuffer, int header1, int h1, int d1, 
            ByteBuffer labelBuffer, int header2, int h2, int d2,
            int numEntries, 
            List<DataPoint> data)
        {
            var b1 = dataBuffer;
            var b2 = labelBuffer;
            if (b1.Read(4) != header1) throw new Exception("File header incorrect");
            if (b2.Read(4) != header2) throw new Exception("File header incorrect");

            if (b1.Read(4) != numEntries) throw new Exception("File header incorrect");
            if (b2.Read(4) != numEntries) throw new Exception("File header incorrect");

            var p1 = 1U;
            for (var i = 0; i < h1; ++i)
                p1 *= b1.Read(4);
            if (p1 != d1) throw new Exception("File header incorrect");

            var p2 = 1U;
            for (var i = 0; i < h2; ++i)
                p2 *= b2.Read(4);
            if (p2 != d2) throw new Exception("File header incorrect");
            Debug.Assert(p2 == 1); // one-hot encoding for now

            // read 
            for (var i = 0; i < numEntries; ++i)
            {
                // input vector
                var input = new Vector(d1);
                for (var j = 0; j < d1; ++j)
                    input[j] = b1.Read()/255.0f; // gray value 0 (white) to 255 (black)

                // output vector
                var output = new Vector(10);
                var label = b2.Read(); // label 0-9
                output[(int)label] = 1.0f;
                
                data.Add(new DataPoint(input,output));
            }
        }
        public static DataSet LoadMNIST(string path)
        {
            var ds = new DataSet();
            ds.Load(
                new ByteBuffer(Path.Combine(path, "train-images.idx3-ubyte")), 0x803,2,28*28,
                new ByteBuffer(Path.Combine(path, "train-labels.idx1-ubyte")), 0x801,0,1,
                60000, 
                ds.TrainingSet
            );
            ds.Load(
                new ByteBuffer(Path.Combine(path, "t10k-images.idx3-ubyte")), 0x803, 2, 28 * 28,
                new ByteBuffer(Path.Combine(path, "t10k-labels.idx1-ubyte")), 0x801, 0, 1,
                10000, 
                ds.TestSet
            );
            return ds;
        }
    }
}
