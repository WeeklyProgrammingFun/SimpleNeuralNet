﻿using System;

namespace NeuralNet.Model
{
    public static class ActivationFunctions
    {
        #region Activation functions

        // Rectified Linear Unit
        public static float ReLU(float x)
        {
            return Math.Max(0, x);
        }
        // Derivative of rectified linear unit
        public static float dReLU(float x)
        {
            return x < 0 ? 0 : 1;
        }

        // Softmax function
        public static Vector Softmax(Vector x)
        {
            // Note often overflows - subtract max from each
            var xm = x[0];
            for (var i = 1; i < x.Size; ++i)
                xm = Math.Max(x[i], xm);


            var ans = new Vector(x.Size);
            var sum = 0.0f;
            for (var i = 0; i < x.Size; ++i)
            {
                ans[i] = (float)Math.Exp(x[i]-xm);
                sum += ans[i];
            }
            return ans * (1.0f / sum);
        }
        // Derivative of Softmax function
        public static Vector dSoftmax(Vector x)
        {
            // Note often overflows - subtract max from each
            var xm = x[0];
            for (var i = 1; i < x.Size; ++i)
                xm = Math.Max(x[i], xm);

            var ans = new Vector(x.Size);
            var sum = 0.0f;
            for (var i = 0; i < x.Size; ++i)
            {
                ans[i] = (float)Math.Exp(x[i]-xm);
                sum += ans[i];
            }

            // ith spot : (-(e^xi)^2 + exi * sum)/(sum*sum)
            var s2 = sum * sum;
            for (var i = 0; i < x.Size; ++i)
            {
                var exi = ans[i];
                ans[i] = exi*(sum - exi) / s2;
            }
            return ans;
        }

            // Logistic function
            public static float Logistic(float x)
        {
            return (float)(1.0 / (1+Math.Exp(-x)));
        }
        // Derivative of Logistic function
        public static float dLogistic(float x)
        {
            return Logistic(x) * (1 - Logistic(x));
        }

        // Identity function f(x)=x
        public static float Identity(float x)
        {
            return x;
        }
        // Derivative of identity function f(x)=x
        public static float dIdentity(float x)
        {
            return 1;
        }

        #endregion

        #region Utility
        // turn a function on a single variable to one operating on vectors
        public static Func<Vector, Vector> Vectorize(Func<float, float> func)
        {
            Func<Vector, Vector> vFunc = v =>
            {
                var v2 = new Vector(v.Size);
                for (var i = 0; i < v.Size; ++i)
                    v2[i] = func(v[i]);
                return v2;
            };
            return vFunc;
        }
        #endregion

    }
}