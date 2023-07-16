using System;
using System.Reflection;
using SkiaSharp;
using Svg.Skia;

namespace Vulcanova.Features.Grades;

public static class GradeShareImageGenerator
{
    private static readonly SKColor BgColor = SKColor.Parse("#F7F7F9");
    private static readonly SKColor PrimaryTextColor = SKColor.Parse("#007AFF");
    private static readonly SKColor SecondaryTextColor = SKColors.Black;

    private static readonly SKTypeface Typeface = SKTypeface.FromFamilyName("SF Pro", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal,
        SKFontStyleSlant.Upright);

    private const int Width = 1080;
    private const int Height = 1920;

    private static readonly SKRect ContentRectangle = SKRect.Create(175, 599, 730, 730);
    private const int ContentRectanglePadding = 36;

    public static byte[] DrawImageForGrade(Grade grade)
    {
        using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

        var canvas = surface.Canvas;
        canvas.Clear(BgColor);

        DrawBackgroundRectangle(canvas);
        DrawBackgroundOrnaments(canvas);
        DrawGradeDetails(canvas, grade);
        DrawForegroundOrnaments(canvas);
        DrawLogo(canvas);

        using var image = surface.Snapshot();
        var bytes = image.Encode(SKEncodedImageFormat.Png, 100);

        return bytes.ToArray();
    }

    private static void DrawBackgroundRectangle(SKCanvas canvas)
    {
        using var rectPaint = new SKPaint();

        rectPaint.Color = SKColors.White;
        rectPaint.ImageFilter = SKImageFilter.CreateDropShadow(0, 16, 32, 32, SKColors.Black.WithAlpha(25));
        rectPaint.IsAntialias = true;

        canvas.DrawRoundRect(ContentRectangle, 36, 36, rectPaint);
    }

    private static void DrawBackgroundOrnaments(SKCanvas canvas)
    {
        var svg = LoadSvg("Vulcanova.Resources.Share.star-ornaments.svg");
        canvas.DrawPicture(svg.Picture, 361, 817);
    }

    private static SKSvg LoadSvg(string name)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)
            ?? throw new ArgumentException("Resource not found", nameof(name));
        var svg = new SKSvg();
        svg.Load(stream);

        return svg;
    }

    private static void DrawGradeDetails(SKCanvas canvas, Grade grade)
    {
        DrawCenteredText(canvas, grade.ContentRaw, 128, 840, PrimaryTextColor);
        DrawCenteredText(canvas, grade.Column.Subject.Name, 64, 1003, PrimaryTextColor, allowDownSizeToFit: true);

        var detailsPosition = 1176;

        if (grade.DateCreated.HasValue)
        {
            DrawCenteredText(canvas, grade.DateCreated.Value.ToShortDateString(), 24, detailsPosition,
                SecondaryTextColor);

            detailsPosition += 24 + 2;
        }

        if (!string.IsNullOrEmpty(grade.Column.Name))
        {
            DrawCenteredText(canvas, grade.Column.Name, 24, detailsPosition, SecondaryTextColor);
        }
    }

    private static void DrawForegroundOrnaments(SKCanvas canvas)
    {
        var svg = LoadSvg("Vulcanova.Resources.Share.arrow-ornament.svg");
        canvas.DrawPicture(svg.Picture, 618, 500);
    }

    private static void DrawCenteredText(SKCanvas canvas, string text, float fontSize, int y, SKColor color, bool allowDownSizeToFit = false)
    {
        using var textPaint = new SKPaint();
        textPaint.Color = color;
        textPaint.IsAntialias = true;
        textPaint.TextSize = fontSize;
        textPaint.Typeface = Typeface;
        textPaint.TextAlign = SKTextAlign.Center;

        if (allowDownSizeToFit)
        {
            while (ContentRectangle.Width - textPaint.MeasureText(text) < ContentRectanglePadding * 2)
            {
                textPaint.TextSize -= 2;
            }
        }

        canvas.DrawText(text, Width / 2f,
            y + textPaint.TextSize, textPaint);
    }

    private static void DrawLogo(SKCanvas canvas)
    {
        const int logoTargetWidth = 42;

        var svg = LoadSvg("Vulcanova.Resources.Icons.vulcanova.svg");
        var scale = logoTargetWidth / svg.Picture!.CullRect.Width;

        var matrix = SKMatrix.Concat(
            SKMatrix.CreateTranslation(ContentRectangle.Left + ContentRectanglePadding,
                ContentRectangle.Top + ContentRectanglePadding),
            SKMatrix.CreateScale(scale, scale));

        canvas.DrawPicture(svg.Picture, ref matrix);

        using var textPaint = new SKPaint();
        textPaint.Color = SecondaryTextColor;
        textPaint.IsAntialias = true;
        textPaint.TextSize = 16;
        textPaint.Typeface = Typeface;

        canvas.DrawText("Vulcanova", ContentRectangle.Left + ContentRectanglePadding + logoTargetWidth + 16,
            ContentRectangle.Top + ContentRectanglePadding + logoTargetWidth / 2f + textPaint.TextSize / 2 - 2,
            textPaint);
    }
}