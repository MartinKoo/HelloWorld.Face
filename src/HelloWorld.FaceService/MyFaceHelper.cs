using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Text;
using ViewFaceCore;
using ViewFaceCore.Configs;
using ViewFaceCore.Core;
using ViewFaceCore.Model;

namespace HelloWorld.FaceService
{
    internal class MyFaceHelper
    {
        private readonly ILogger m_logger;

        private FaceDetector NewFaceDetector => new FaceDetector();

        private FaceLandmarker NewFaceLandmarker => new FaceLandmarker(new FaceLandmarkConfig { MarkType = MarkType.Light });

        private FaceRecognizer NewFaceRecognizer => new FaceRecognizer(new FaceRecognizeConfig { FaceType = FaceType.Light });

        public MyFaceHelper(ILogger logger)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            m_logger = logger;
        }

        public float[] GetFaceFeature(SKBitmap image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var faceInfos = NewFaceDetector.Detect(image);

            m_logger.LogInformation(GetFaceInfosMessage(faceInfos));

            if (faceInfos.Length > 0)
            {
                if (faceInfos.Length > 1)
                {
                    throw new Exception("检测到多张人脸！");
                }

                return NewFaceRecognizer.Extract(image, NewFaceLandmarker.Mark(image, faceInfos[0]));
            }

            return null;
        }

        private string GetFaceInfosMessage(FaceInfo[] faceInfos)
        {
            if (faceInfos.Length > 0)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"检测到的人脸数量：{faceInfos.Length}");
                sb.AppendLine("人脸信息：");
                sb.AppendLine("序号\t人脸置信度\t位置X\t位置Y\t宽度\t高度");

                for (int i = 0; i < faceInfos.Length; i++)
                {
                    var x = faceInfos[i];

                    sb.AppendLine($"{i + 1}\t{x.Score}\t{x.Location.X}\t{x.Location.Y}\t{x.Location.Width}\t{x.Location.Height}");
                }

                return sb.ToString();
            }

            return "未检测到人脸";
        }

        public bool IsSamePerson(SKBitmap image, float[] targetFaceFeature)
        {
            return IsSamePerson(GetFaceFeature(image), targetFaceFeature);
        }

        public bool IsSamePerson(float[] currentFaceFeature, float[] targetFaceFeature)
        {
            if (currentFaceFeature is null)
            {
                throw new ArgumentNullException(nameof(currentFaceFeature));
            }

            if (targetFaceFeature is null)
            {
                throw new ArgumentNullException(nameof(targetFaceFeature));
            }

            var x = NewFaceRecognizer.Compare(currentFaceFeature, targetFaceFeature);
            var y = NewFaceRecognizer.IsSelf(x);

            m_logger.LogInformation($"相似度 = {x}，判定：{(y ? "是" : "不是")}同一个人");

            return y;
        }
    }
}
