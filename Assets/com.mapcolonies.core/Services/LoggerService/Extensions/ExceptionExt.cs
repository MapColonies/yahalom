using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace com.mapcolonies.core.Services.LoggerService.Extensions
{
    public static class ExceptionExt
    {
        private const int DownBorder = 64;
        private const int UpBorderOffset = 64;

        private static void AddNestedType(StringBuilder sb, Type type)
        {
            if (type.IsNested)
            {
                AddNestedType(sb, type.DeclaringType);
                sb.Append("+");
            }

            sb.Append(type.Name);
        }

        public static string UnityMessageWithStack(this Exception exception, bool withFiles = true)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            try
            {
                return UnityMessageWithStackInternal(exception, withFiles);
            }
            catch
            {
                return $"{exception.Message}\r\n{exception.StackTrace}";
            }
        }

        private static string UnityMessageWithStackInternal(this Exception exception, bool withFiles = true)
        {
            StackTrace trace = new StackTrace(exception, withFiles);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(exception.Message);

            AppendStackFrames(sb, trace);

            return sb.ToString();
        }

        private static void AppendStackFrames(StringBuilder sb, StackTrace trace)
        {
            bool hasSkippedFrames = false;

            for (int i = 0; i < trace.FrameCount; i++)
            {
                if (i > DownBorder && i < trace.FrameCount - UpBorderOffset)
                {
                    if (!hasSkippedFrames)
                    {
                        sb.AppendLine("-------- Cut very long stack --------");
                        hasSkippedFrames = true;
                    }

                    continue;
                }

                StackFrame frame = trace.GetFrame(i);
                AppendFrameDetails(sb, frame);
            }
        }

        private static void AppendFrameDetails(StringBuilder sb, StackFrame frame)
        {
            MethodBase method = frame.GetMethod();
            Type type = method?.DeclaringType;

            if (type != null)
            {
                AppendNamespaceAndType(sb, type);
            }

            sb.Append(":");
            sb.Append(method?.Name);
            sb.Append(" (");
            AppendMethodParameters(sb, method);
            sb.Append(")");

            if (frame.GetFileName() != null)
            {
                AppendFileDetails(sb, frame);
            }

            sb.AppendLine();
        }

        private static void AppendNamespaceAndType(StringBuilder sb, Type type)
        {
            if (!string.IsNullOrEmpty(type.Namespace))
            {
                sb.Append(type.Namespace);
                sb.Append(".");
            }

            AddNestedType(sb, type);
        }

        private static void AppendMethodParameters(StringBuilder sb, MethodBase method)
        {
            ParameterInfo[] parameters = method?.GetParameters();
            if (parameters == null) return;

            for (int i = 0; i < parameters.Length; i++)
            {
                sb.Append(parameters[i].ParameterType.Name);

                if (i < parameters.Length - 1)
                {
                    sb.Append(", ");
                }
            }
        }

        private static void AppendFileDetails(StringBuilder sb, StackFrame frame)
        {
            string file = frame.GetFileName();
            bool isAsset = TryFormatFilePath(ref file);

            sb.Append("(at ");

            if (isAsset && Log4NetHandler.UnityVersion.Major >= 2020 && Application.isEditor)
            {
                sb.Append("<a href=\"");
                sb.Append(file);
                sb.Append("\" line=\"");
                sb.Append(frame.GetFileLineNumber());
                sb.Append("\">");
            }

            sb.Append(file);
            sb.Append(":");
            sb.Append(frame.GetFileLineNumber());

            if (isAsset && Log4NetHandler.UnityVersion.Major >= 2020 && Application.isEditor)
            {
                sb.Append("</a>");
            }

            sb.Append(")");
        }

        private static bool TryFormatFilePath(ref string file)
        {
            bool isAsset = false;

            if (!Application.isEditor || string.IsNullOrEmpty(file)) return false;

            file = Path.GetFullPath(file);
            string applicationDataPath = string.IsNullOrEmpty(Log4NetHandler.ApplicationDataPath)
                ? "Assets"
                : Log4NetHandler.ApplicationDataPath;
            applicationDataPath = Path.GetFullPath(applicationDataPath);

            string projectPath = Path.GetDirectoryName(applicationDataPath);

            if (!string.IsNullOrEmpty(projectPath) && file.StartsWith(Path.GetFullPath(projectPath)))
            {
                isAsset = true;
                file = file.Remove(0, projectPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            file = file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return isAsset;
        }
    }
}
