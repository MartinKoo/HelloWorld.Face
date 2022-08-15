using System.Text;

namespace HelloWorld.FaceService
{
    public static class MyFaceFeatureUtil
    {
        public static string ToString(float[] data)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }

                sb.Append(data[i]);
            }

            return sb.ToString();
        }

        public static float[] ToArray(string data)
        {
            string[] array = data.Split(',');

            float[] result = new float[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = float.Parse(array[i]);
            }

            return result;
        }
    }
}
