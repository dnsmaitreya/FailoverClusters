using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using MS.Internal.FailoverClusters.Framework;

namespace FailoverClusters.Framework;

public class Icon2 : IEnumerable<Icon>, IEnumerable
{
	[Serializable]
	public struct Enumerator : IEnumerator<Icon>, IDisposable, IEnumerator
	{
		[NonSerialized]
		private readonly Icon2 list;

		private int index;

		private Icon current;

		public Icon Current => current;

		object IEnumerator.Current => Current;

		internal Enumerator(Icon2 list)
		{
			this.list = list;
			index = 0;
			current = null;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (index < list.Count)
			{
				current = list[index];
				index++;
				return true;
			}
			index = list.Count + 1;
			current = null;
			return false;
		}

		void IEnumerator.Reset()
		{
			index = 0;
			current = null;
		}
	}

	private sealed class IconImage
	{
		private ImageEncoder encoder;

		public Size Size => new Size((int)encoder.Header.biWidth, (int)(encoder.Header.biHeight / 2u));

		public PixelFormat PixelFormat => encoder.Header.biBitCount switch
		{
			1 => PixelFormat.Format1bppIndexed, 
			4 => PixelFormat.Format4bppIndexed, 
			8 => PixelFormat.Format8bppIndexed, 
			16 => PixelFormat.Format16bppRgb565, 
			24 => PixelFormat.Format24bppRgb, 
			32 => PixelFormat.Format32bppArgb, 
			_ => PixelFormat.Undefined, 
		};

		public Icon Icon => encoder.Icon;

		public Bitmap Transparent => Icon.ToBitmap();

		public ImageEncoder Encoder => encoder;

		internal IconImage()
		{
			encoder = new BMPEncoder();
		}

		internal IconImage(Stream stream, int resourceSize)
		{
			Read(stream, resourceSize);
		}

		public unsafe void Set(Bitmap bitmap, Bitmap bitmapMask, Color transparentColor)
		{
			Bitmap bitmap2 = (Bitmap)bitmap.Clone();
			Bitmap bitmap3 = ((bitmapMask != null) ? ((Bitmap)bitmapMask.Clone()) : null);
			try
			{
				if (bitmap2.PixelFormat != PixelFormat.Format1bppIndexed)
				{
					bitmap2.RotateFlip(RotateFlipType.Rotate180FlipX);
				}
				else
				{
					FlipYBitmap(bitmap2);
				}
				if (bitmap3 != null)
				{
					FlipYBitmap(bitmap3);
				}
				if (bitmap3 != null && (bitmap2.Size != bitmap3.Size || bitmap3.PixelFormat != PixelFormat.Format1bppIndexed))
				{
					throw new InvalidOperationException("The mask for this bitmap is invalid");
				}
				NativeMethods.RGBQUAD[] array = RGBQUADFromColorArray(bitmap2);
				NativeMethods.BITMAPINFOHEADER bITMAPINFOHEADER = default(NativeMethods.BITMAPINFOHEADER);
				bITMAPINFOHEADER.biSize = (uint)sizeof(NativeMethods.BITMAPINFOHEADER);
				bITMAPINFOHEADER.biWidth = (uint)bitmap2.Width;
				bITMAPINFOHEADER.biHeight = (uint)(bitmap2.Height * 2);
				bITMAPINFOHEADER.biPlanes = 1;
				bITMAPINFOHEADER.biBitCount = (ushort)BitsFromPixelFormat(bitmap2.PixelFormat);
				bITMAPINFOHEADER.biCompression = IconImageFormat.BMP;
				bITMAPINFOHEADER.biXPelsPerMeter = 0;
				bITMAPINFOHEADER.biYPelsPerMeter = 0;
				bITMAPINFOHEADER.biClrUsed = (uint)array.Length;
				bITMAPINFOHEADER.biClrImportant = 0u;
				NativeMethods.BITMAPINFOHEADER header = bITMAPINFOHEADER;
				encoder.Header = header;
				encoder.Colors = array;
				BitmapData bitmapData = bitmap2.LockBits(new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), ImageLockMode.ReadOnly, bitmap2.PixelFormat);
				IntPtr scan = bitmapData.Scan0;
				encoder.XOR = new byte[Math.Abs(bitmapData.Stride) * bitmapData.Height];
				Marshal.Copy(scan, encoder.XOR, 0, encoder.XOR.Length);
				bitmap2.UnlockBits(bitmapData);
				header.biSizeImage = (uint)encoder.XOR.Length;
				if (bitmap3 == null)
				{
					Bitmap bitmap4 = new Bitmap(bitmap2.Width, bitmap2.Height, PixelFormat.Format1bppIndexed);
					BitmapData bitmapData2 = bitmap4.LockBits(new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), ImageLockMode.ReadWrite, bitmap4.PixelFormat);
					_ = bitmapData2.Scan0;
					encoder.AND = new byte[Math.Abs(bitmapData2.Stride) * bitmapData2.Height];
					int num = Math.Abs(bitmapData.Stride);
					int num2 = Math.Abs(bitmapData2.Stride);
					int num3 = BitsFromPixelFormat(bitmap2.PixelFormat);
					if (num3 == 24)
					{
						transparentColor = Color.FromArgb(0, transparentColor.R, transparentColor.G, transparentColor.B);
					}
					for (int i = 0; i < bitmapData.Height; i++)
					{
						int num4 = num2 * i;
						int num5 = num * i;
						for (int j = 0; j < bitmapData.Width; j++)
						{
							switch (num3)
							{
							case 1:
								encoder.AND[(j >> 3) + num5] = encoder.XOR[(j >> 3) + num5];
								break;
							case 4:
							{
								int num7 = encoder.XOR[(j >> 1) + num5];
								if (CompareRGBQUADToColor(encoder.Colors[((j & 1) == 0) ? (num7 >> 4) : (num7 & 0xF)], transparentColor))
								{
									encoder.AND[(j >> 3) + num4] |= (byte)(128 >> (j & 7));
									encoder.XOR[(j >> 1) + num5] &= (byte)(((j & 1) == 0) ? 15 : 240);
								}
								break;
							}
							case 8:
							{
								int num7 = encoder.XOR[j + num5];
								if (CompareRGBQUADToColor(encoder.Colors[num7], transparentColor))
								{
									encoder.AND[(j >> 3) + num4] |= (byte)(128 >> (j & 7));
									encoder.XOR[j + num5] = 0;
								}
								break;
							}
							case 16:
								throw new NotSupportedException("16 bpp images are not supported for Icons");
							case 24:
							{
								int num6 = j * 3;
								if (Color.FromArgb(0, encoder.XOR[num6 + num5], encoder.XOR[num6 + num5 + 1], encoder.XOR[num6 + num5 + 2]) == transparentColor)
								{
									encoder.AND[(j >> 3) + num4] |= (byte)(128 >> (j & 7));
								}
								break;
							}
							case 32:
								if (transparentColor == Color.Transparent)
								{
									if (encoder.XOR[(j << 2) + num5 + 3] == 0)
									{
										encoder.AND[(j >> 3) + num4] |= (byte)(128 >> (j & 7));
									}
								}
								else if (encoder.XOR[(j << 2) + num5] == transparentColor.B && encoder.XOR[(j << 2) + num5 + 1] == transparentColor.G && encoder.XOR[(j << 2) + num5 + 2] == transparentColor.R)
								{
									encoder.AND[(j >> 3) + num4] |= (byte)(128 >> (j & 7));
									encoder.XOR[(j << 2) + num5] = 0;
									encoder.XOR[(j << 2) + num5 + 1] = 0;
									encoder.XOR[(j << 2) + num5 + 2] = 0;
								}
								else
								{
									encoder.XOR[(j << 2) + num5 + 3] = byte.MaxValue;
								}
								break;
							}
						}
					}
					bitmap4.UnlockBits(bitmapData2);
				}
				else
				{
					BitmapData bitmapData3 = bitmap3.LockBits(new Rectangle(0, 0, bitmap3.Width, bitmap3.Height), ImageLockMode.ReadOnly, bitmap3.PixelFormat);
					IntPtr scan2 = bitmapData3.Scan0;
					encoder.AND = new byte[Math.Abs(bitmapData3.Stride) * bitmapData3.Height];
					Marshal.Copy(scan2, encoder.AND, 0, encoder.AND.Length);
					bitmap3.UnlockBits(bitmapData3);
				}
			}
			finally
			{
				bitmap2?.Dispose();
				bitmap3?.Dispose();
			}
		}

		internal void Read(Stream stream, int resourceSize)
		{
			switch (GetIconImageFormat(stream))
			{
			case IconImageFormat.BMP:
				encoder = new BMPEncoder();
				encoder.Read(stream, resourceSize);
				break;
			case IconImageFormat.PNG:
				encoder = new PNGEncoder();
				encoder.Read(stream, resourceSize);
				break;
			}
		}

		private unsafe static IconImageFormat GetIconImageFormat(Stream stream)
		{
			long position = stream.Position;
			try
			{
				BinaryReader binaryReader = new BinaryReader(stream);
				_ = new byte[sizeof(NativeMethods.BITMAPINFOHEADER)];
				switch (binaryReader.ReadByte())
				{
				case 40:
					return IconImageFormat.BMP;
				case 137:
					if (binaryReader.ReadInt16() == 20048)
					{
						return IconImageFormat.PNG;
					}
					break;
				}
				return IconImageFormat.UNKNOWN;
			}
			finally
			{
				stream.Position = position;
			}
		}
	}

	private abstract class ImageEncoder
	{
		private NativeMethods.BITMAPINFOHEADER memberHeader;

		private NativeMethods.RGBQUAD[] memberColors;

		private byte[] memberXor;

		private byte[] memberAnd;

		public unsafe virtual Icon Icon
		{
			get
			{
				MemoryStream memoryStream = new MemoryStream();
				NativeMethods.ICONDIR initalizated = NativeMethods.ICONDIR.Initalizated;
				initalizated.idCount = 1;
				initalizated.Write(memoryStream);
				NativeMethods.ICONDIRENTRY iCONDIRENTRY = default(NativeMethods.ICONDIRENTRY);
				iCONDIRENTRY.bColorCount = (byte)memberHeader.biClrUsed;
				iCONDIRENTRY.bHeight = (byte)(memberHeader.biHeight / 2u);
				iCONDIRENTRY.bReserved = 0;
				iCONDIRENTRY.bWidth = (byte)memberHeader.biWidth;
				iCONDIRENTRY.dwBytesInRes = (uint)(sizeof(NativeMethods.BITMAPINFOHEADER) + sizeof(NativeMethods.RGBQUAD) * ColorsInPalette + memberXor.Length + memberAnd.Length);
				iCONDIRENTRY.dwImageOffset = (uint)(sizeof(NativeMethods.ICONDIR) + sizeof(NativeMethods.ICONDIRENTRY));
				iCONDIRENTRY.wBitCount = memberHeader.biBitCount;
				iCONDIRENTRY.wPlanes = memberHeader.biPlanes;
				iCONDIRENTRY.Write(memoryStream);
				memoryStream.Seek(iCONDIRENTRY.dwImageOffset, SeekOrigin.Begin);
				memberHeader.Write(memoryStream);
				byte[] array = new byte[sizeof(NativeMethods.RGBQUAD) * ColorsInPalette];
				GCHandle gCHandle = GCHandle.Alloc(memberColors, GCHandleType.Pinned);
				Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, 0, array.Length);
				gCHandle.Free();
				memoryStream.Write(array, 0, array.Length);
				memoryStream.Write(memberXor, 0, memberXor.Length);
				memoryStream.Write(memberAnd, 0, memberAnd.Length);
				memoryStream.Position = 0L;
				Icon result = new Icon(memoryStream, iCONDIRENTRY.bWidth, iCONDIRENTRY.bHeight);
				memoryStream.Dispose();
				return result;
			}
		}

		public NativeMethods.BITMAPINFOHEADER Header
		{
			get
			{
				return memberHeader;
			}
			set
			{
				memberHeader = value;
			}
		}

		public NativeMethods.RGBQUAD[] Colors
		{
			get
			{
				return memberColors;
			}
			set
			{
				memberColors = value;
			}
		}

		public byte[] XOR
		{
			get
			{
				return memberXor;
			}
			set
			{
				memberHeader.biSizeImage = (uint)value.Length;
				memberXor = value;
			}
		}

		public byte[] AND
		{
			get
			{
				return memberAnd;
			}
			set
			{
				memberAnd = value;
			}
		}

		public virtual int ColorsInPalette
		{
			get
			{
				if (memberHeader.biClrUsed == 0)
				{
					if (memberHeader.biBitCount > 8)
					{
						return 0;
					}
					return 1 << (int)memberHeader.biBitCount;
				}
				return (int)memberHeader.biClrUsed;
			}
		}

		public unsafe virtual int ImageSize => sizeof(NativeMethods.BITMAPINFOHEADER) + sizeof(NativeMethods.RGBQUAD) * ColorsInPalette + memberXor.Length + memberAnd.Length;

		public abstract IconImageFormat IconImageFormat { get; }

		public abstract void Read(Stream stream, int resourceSize);

		public void CopyFrom(ImageEncoder encoder)
		{
			memberHeader = encoder.memberHeader;
			memberColors = encoder.memberColors;
			memberXor = encoder.memberXor;
			memberAnd = encoder.memberAnd;
		}
	}

	private class PNGEncoder : ImageEncoder
	{
		public override IconImageFormat IconImageFormat => IconImageFormat.PNG;

		public override int ImageSize
		{
			get
			{
				MemoryStream memoryStream = new MemoryStream();
				Icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
				return (int)memoryStream.Length;
			}
		}

		public override void Read(Stream stream, int resourceSize)
		{
			byte[] array = new byte[resourceSize];
			stream.Read(array, 0, array.Length);
			Bitmap bitmap = new Bitmap(new MemoryStream(array));
			IconImage iconImage = new IconImage();
			iconImage.Set(bitmap, null, Color.Transparent);
			bitmap.Dispose();
			CopyFrom(iconImage.Encoder);
		}
	}

	private class BMPEncoder : ImageEncoder
	{
		public override IconImageFormat IconImageFormat => IconImageFormat.BMP;

		public unsafe override void Read(Stream stream, int resourceSize)
		{
			NativeMethods.BITMAPINFOHEADER header = default(NativeMethods.BITMAPINFOHEADER);
			header.Read(stream);
			base.Header = header;
			base.Colors = new NativeMethods.RGBQUAD[ColorsInPalette];
			byte[] array = new byte[base.Colors.Length * sizeof(NativeMethods.RGBQUAD)];
			stream.Read(array, 0, array.Length);
			GCHandle gCHandle = GCHandle.Alloc(base.Colors, GCHandleType.Pinned);
			Marshal.Copy(array, 0, gCHandle.AddrOfPinnedObject(), array.Length);
			gCHandle.Free();
			int num = (int)((base.Header.biWidth * base.Header.biBitCount + 31) & -32) >> 3;
			base.XOR = new byte[num * (base.Header.biHeight / 2u)];
			stream.Read(base.XOR, 0, base.XOR.Length);
			num = (int)((base.Header.biWidth + 31) & -32) >> 3;
			base.AND = new byte[num * (base.Header.biHeight / 2u)];
			stream.Read(base.AND, 0, base.AND.Length);
		}
	}

	private class IconFormat
	{
		public bool IsRecognizedFormat(Stream stream)
		{
			stream.Position = 0L;
			try
			{
				NativeMethods.ICONDIR iCONDIR = new NativeMethods.ICONDIR(stream);
				if (iCONDIR.idReserved != 0)
				{
					return false;
				}
				if (iCONDIR.idType != 1)
				{
					return false;
				}
				return true;
			}
			catch (Exception)
			{
			}
			return false;
		}

		public unsafe void Load(Icon2 icon2, Stream stream)
		{
			stream.Position = 0L;
			NativeMethods.ICONDIR iCONDIR = new NativeMethods.ICONDIR(stream);
			if (iCONDIR.idReserved != 0)
			{
				throw new InvalidOperationException("Invalid Icon file Exception");
			}
			if (iCONDIR.idType != 1)
			{
				throw new InvalidOperationException("Invalid Icon file Exception");
			}
			int num = sizeof(NativeMethods.ICONDIR);
			for (int i = 0; i < iCONDIR.idCount; i++)
			{
				stream.Seek(num, SeekOrigin.Begin);
				NativeMethods.ICONDIRENTRY entry = new NativeMethods.ICONDIRENTRY(stream);
				stream.Seek(CheckAndRepairEntry(entry).dwImageOffset, SeekOrigin.Begin);
				icon2.Add(new IconImage(stream, (int)(stream.Length - stream.Position)));
				num += sizeof(NativeMethods.ICONDIRENTRY);
			}
		}

		private unsafe static NativeMethods.ICONDIRENTRY CheckAndRepairEntry(NativeMethods.ICONDIRENTRY entry)
		{
			if (entry.wBitCount == 0)
			{
				int num = (ushort)entry.dwBytesInRes - sizeof(NativeMethods.BITMAPINFOHEADER);
				int num2 = (((entry.bWidth + 31) & -32) >> 3) * entry.bHeight;
				num -= num2;
				byte[] array = new byte[6] { 1, 4, 8, 16, 24, 32 };
				for (int i = 0; i <= 5; i++)
				{
					int num3 = ((entry.bWidth * array[i] + 31) & -32) >> 3;
					int num4 = entry.bHeight * num3;
					if (((array[i] <= 8) ? ((1 << (int)array[i]) * 4) : 0) + num4 == num)
					{
						entry.wBitCount = array[i];
						break;
					}
				}
			}
			if (entry.wBitCount < 8 && entry.bColorCount == 0)
			{
				entry.bColorCount = (byte)(1 << (int)entry.wBitCount);
			}
			if (entry.wPlanes == 0)
			{
				entry.wPlanes = 1;
			}
			return entry;
		}
	}

	private readonly Icon icon;

	private readonly List<IconImage> iconImages = new List<IconImage>();

	private string name = string.Empty;

	public Icon NativeIcon => icon;

	public int Count => iconImages.Count;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value ?? string.Empty;
		}
	}

	public Icon this[int index] => iconImages[index].Icon;

	public Icon2(Icon icon)
	{
		if (icon == null)
		{
			throw new NullReferenceException("icon");
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			icon.Save(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			Load(memoryStream);
		}
		this.icon = icon;
	}

	public Bitmap Get(int width, int height, PixelFormat pixelFormat = PixelFormat.Undefined, bool createIfNotExist = true)
	{
		IconImage iconImage = null;
		IconImage iconImage2 = null;
		foreach (IconImage iconImage3 in iconImages)
		{
			if (iconImage3.Size.Width == width && iconImage3.Size.Height == height)
			{
				if (pixelFormat == iconImage3.PixelFormat)
				{
					return iconImage3.Transparent;
				}
				if (pixelFormat == PixelFormat.Undefined)
				{
					if (iconImage == null)
					{
						iconImage = iconImage3;
					}
					else if (iconImage3.PixelFormat > iconImage.PixelFormat)
					{
						iconImage = iconImage3;
					}
				}
			}
			if (iconImage2 == null)
			{
				iconImage2 = iconImage3;
				continue;
			}
			int num = width - iconImage2.Size.Width + (height - iconImage2.Size.Height);
			int num2 = width - iconImage3.Size.Width + (height - iconImage3.Size.Height);
			if (num2 == num)
			{
				if (iconImage3.PixelFormat > iconImage2.PixelFormat)
				{
					iconImage2 = iconImage3;
				}
			}
			else if (Math.Abs(num2) < Math.Abs(num))
			{
				iconImage2 = iconImage3;
			}
		}
		if (iconImage != null)
		{
			return iconImage.Transparent;
		}
		if (createIfNotExist && iconImage2 != null)
		{
			if (width == iconImage2.Size.Width && height == iconImage2.Size.Height)
			{
				return iconImage2.Transparent;
			}
			return CreateSmoothBitmap(iconImage2.Transparent, width, height);
		}
		return null;
	}

	public IEnumerator<Icon> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private void Load(Stream stream)
	{
		IconFormat iconFormat = new IconFormat();
		if (!iconFormat.IsRecognizedFormat(stream))
		{
			throw new InvalidOperationException("Invalid Icon");
		}
		iconFormat.Load(this, stream);
	}

	private void Add(IconImage iconImage)
	{
		iconImages.Add(iconImage);
	}

	private static Bitmap CreateSmoothBitmap(Bitmap bmp, int width, int height)
	{
		Bitmap bitmap = new Bitmap(width, height);
		using Graphics graphics = Graphics.FromImage(bitmap);
		graphics.CompositingQuality = CompositingQuality.HighQuality;
		graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		graphics.SmoothingMode = SmoothingMode.HighQuality;
		graphics.DrawImage(bmp, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
		return bitmap;
	}

	public override string ToString()
	{
		return Name;
	}

	internal static bool CompareRGBQUADToColor(NativeMethods.RGBQUAD rgbQuad, Color color)
	{
		if (rgbQuad.rgbRed == color.R && rgbQuad.rgbGreen == color.G)
		{
			return rgbQuad.rgbBlue == color.B;
		}
		return false;
	}

	internal static NativeMethods.RGBQUAD[] RGBQUADFromColorArray(Bitmap bmp)
	{
		int num = BitsFromPixelFormat(bmp.PixelFormat);
		NativeMethods.RGBQUAD[] array = new NativeMethods.RGBQUAD[(num <= 8) ? (1 << num) : 0];
		Color[] entries = bmp.Palette.Entries;
		for (int i = 0; i < entries.Length; i++)
		{
			array[i].rgbRed = entries[i].R;
			array[i].rgbGreen = entries[i].G;
			array[i].rgbBlue = entries[i].B;
		}
		return array;
	}

	private unsafe static void FlipYBitmap(Bitmap bitmap)
	{
		if (bitmap.PixelFormat != PixelFormat.Format1bppIndexed)
		{
			return;
		}
		BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
		byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
		fixed (byte* ptr2 = new byte[bitmapData.Stride])
		{
			for (int i = 0; i < bitmap.Height / 2; i++)
			{
				NativeMethods.CopyMemory(ptr2, ptr + i * bitmapData.Stride, bitmapData.Stride);
				NativeMethods.CopyMemory(ptr + i * bitmapData.Stride, ptr + (bitmap.Height - 1 - i) * bitmapData.Stride, bitmapData.Stride);
				NativeMethods.CopyMemory(ptr + (bitmap.Height - 1 - i) * bitmapData.Stride, ptr2, bitmapData.Stride);
			}
		}
		bitmap.UnlockBits(bitmapData);
	}

	private static int BitsFromPixelFormat(PixelFormat pixelFormat)
	{
		switch (pixelFormat)
		{
		case PixelFormat.Format1bppIndexed:
			return 1;
		case PixelFormat.Format4bppIndexed:
			return 4;
		case PixelFormat.Format8bppIndexed:
			return 8;
		case PixelFormat.Format16bppRgb555:
		case PixelFormat.Format16bppRgb565:
		case PixelFormat.Format16bppArgb1555:
		case PixelFormat.Format16bppGrayScale:
			return 16;
		case PixelFormat.Format24bppRgb:
			return 24;
		case PixelFormat.Format32bppRgb:
		case PixelFormat.Format32bppPArgb:
		case PixelFormat.Format32bppArgb:
			return 32;
		case PixelFormat.Format64bppPArgb:
		case PixelFormat.Format64bppArgb:
			return 64;
		default:
			return 0;
		}
	}
}

