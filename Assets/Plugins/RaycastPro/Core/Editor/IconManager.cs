#if UNITY_EDITOR
namespace RaycastPro.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using RaycastPro;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Icon manager.
    /// </summary>
    public static class IconManager
    {
        /// <summary>
        /// Read all bytes in this stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>All bytes in the stream.</returns>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            long originalPosition = 0;
 
            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }
 
            try
            {
                var readBuffer = new byte[4096];
 
                var totalBytesRead = 0;
                int bytesRead;
 
                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;
 
                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }
 
                var buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
 
        private static readonly Dictionary<string, Texture2D> _embeddedIcons = new Dictionary<string, Texture2D>();
        
        /// <summary>
        /// Get the embedded icon with the given resource name.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <returns>The embedded icon with the given resource name.</returns>
        public static Texture2D GetEmbeddedIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            if (_embeddedIcons.TryGetValue(resourceName, out var icon) && icon != null) return icon;
            
            byte[] iconBytes;
            using (var stream = assembly.GetManifestResourceStream(resourceName)) iconBytes = stream.ReadAllBytes();
            icon = new Texture2D(128, 128);
            icon.LoadImage(iconBytes);
            icon.name = resourceName;
 
            _embeddedIcons[resourceName] = icon;

            return icon;
        }
 
        /// <summary>
        /// Set the icon for this object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="texture">The icon.</param>
        public static void SetIcon(this Object obj, Texture2D texture)
        {
            try
            {
#if UNITY_2021_2_OR_NEWER
                EditorGUIUtility.SetIconForObject(obj, texture);
#else
                var ty = typeof(EditorGUIUtility);
                var method = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);

                method.Invoke(null, new object[] {obj, texture});
#endif

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Set the icon for this object from an embedded resource.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="resourceName"></param>
        public static void SetIcon(this Object obj, string resourceName) =>  SetIcon(obj, GetEmbeddedIcon(resourceName));
        /// <summary>
        /// Get the icon for this object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The icon for this object.</returns>
        public static Texture2D GetIcon(this Object obj)
        {
#if UNITY_2021_2_OR_NEWER && !UNITY_2021_1 && !UNITY_2021_2
            return EditorGUIUtility.GetIconForObject(obj);
#else
            var ty = typeof(EditorGUIUtility);
            var mi = ty.GetMethod("GetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
            return mi.Invoke(null, new object[] { obj }) as Texture2D;
#endif
        }
 
        /// <summary>
        /// Remove this icon's object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void RemoveIcon(this Object obj) =>  SetIcon(obj, (Texture2D) null);
        public static Dictionary<Type, Texture2D> GetIcons()
        {
            var dict = new Dictionary<Type, Texture2D>();
            
            var icons = GetResources();
            
            var lookup = icons.ToLookup(I => I.name);

            const string prefix = "Icon_";

            var CoreTypes = typeof(RaycastCore).GetInheritedTypes();

            foreach (var type in CoreTypes)
            {
                var icon = GetIconFromType(type, lookup, prefix);

                if (icon) dict.Add(type, icon);
            }
            return dict;
        }
        public static Texture2D GetIconFromType(Type type, ILookup<string, Texture2D> lookup, string prefix)
        {
            var textureArray = lookup[$"{prefix}{type.Name}"].ToArray();

            var texture = textureArray.Length > 0 ? textureArray[0] : null;

            return texture;
        }

        public static Texture2D GetIconFromName(string name)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(RCProPanel.ResourcePath + $"/{name}.png");

            return texture;
        }
        
        //private static string Resource_Path = "Assets/RaycastPro/Resources";
        public static Texture2D GetHeader() => AssetDatabase.LoadAssetAtPath<Texture2D>(RCProPanel.ResourcePath + "/RaycastPro_Header.png");

        public static IEnumerable<Texture2D> GetResources()
        {
            var files = Directory.GetFiles(RCProPanel.ResourcePath, "*.png", SearchOption.TopDirectoryOnly);
            
            return files.Select(AssetDatabase.LoadAssetAtPath<Texture2D>);
        }
    }
}

#endif