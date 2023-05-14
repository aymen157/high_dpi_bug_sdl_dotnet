using SDL2;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace highdpi_bug
{
    internal class Program
    {

        [DllImport("SHCore.dll", SetLastError = true)]
        private static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

        [DllImport("SHCore.dll", SetLastError = true)]
        private static extern void GetProcessDpiAwareness(IntPtr hprocess, out PROCESS_DPI_AWARENESS awareness);

        private enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }

        static void Main(string[] args)
        {

            // enable this to see the difference !
            // this is how SDL should work. and how it works on other frameworks.
            // SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);

            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            SDL_ttf.TTF_Init();

            string text = "Hello, SDL!";

            // Create a window
            IntPtr window = SDL.SDL_CreateWindow("SDL Text", 
                SDL.SDL_WINDOWPOS_UNDEFINED, 
                SDL.SDL_WINDOWPOS_UNDEFINED,
                800, 600, 
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
                | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL
                | SDL.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI // this has no effect on .Net applications, i suspect its because of the boilerplate?
                    );

            // Create a renderer
            IntPtr renderer = SDL.SDL_CreateRenderer(window, -1, 0);

            // Create a font
            IntPtr font = SDL_ttf.TTF_OpenFont("./Roboto-Regular.ttf", 24);

            // Set the text color
            SDL.SDL_Color textColor = new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 };

            // Create a surface with the text
            IntPtr surface = SDL_ttf.TTF_RenderText_Solid(font, text, textColor);

            // Create a texture from the surface
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer, surface);

            // Clear the renderer
            SDL.SDL_RenderClear(renderer);


            int textWidth, textHeight;
            SDL_ttf.TTF_SizeText(font, text, out textWidth, out textHeight);
            SDL.SDL_Rect destRect = new SDL.SDL_Rect { x = 100, y = 100, w = textWidth, h = textHeight };

            // Copy the texture to the renderer
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref destRect);

            // Render the changes
            SDL.SDL_RenderPresent(renderer);

            while (true)
                ;

            // Delay to show the window
            SDL.SDL_Delay(2000);

            // Clean up resources
            SDL.SDL_DestroyTexture(texture);
            SDL.SDL_FreeSurface(surface);
            SDL_ttf.TTF_CloseFont(font);
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}