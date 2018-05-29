using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Device device;
        Mesh[] meshes = new Mesh[1];
        Camera camera = new Camera();

        public MainWindow()
        {
            InitializeComponent();


            // Choose the back buffer resolution here
            WriteableBitmap bmp = new WriteableBitmap(640, 480, 1, 1, PixelFormats.Bgra32, null);

            // Our Image XAML control
            frontBuffer.Source = bmp;

            device = new Device(bmp);

            Mesh mesh = new Mesh("Cube", 8, 12);
            mesh.Vertices[0] = new Vector3(-1, 1, 1);
            mesh.Vertices[1] = new Vector3(1, 1, 1);
            mesh.Vertices[2] = new Vector3(-1, -1, 1);
            mesh.Vertices[3] = new Vector3(1, -1, 1);
            mesh.Vertices[4] = new Vector3(-1, 1, -1);
            mesh.Vertices[5] = new Vector3(1, 1, -1);
            mesh.Vertices[6] = new Vector3(1, -1, -1);
            mesh.Vertices[7] = new Vector3(-1, -1, -1);

            mesh.Faces[0] = new Face { A = 0, B = 1, C = 2 };
            mesh.Faces[1] = new Face { A = 1, B = 2, C = 3 };
            mesh.Faces[2] = new Face { A = 1, B = 3, C = 6 };
            mesh.Faces[3] = new Face { A = 1, B = 5, C = 6 };
            mesh.Faces[4] = new Face { A = 0, B = 1, C = 4 };
            mesh.Faces[5] = new Face { A = 1, B = 4, C = 5 };

            mesh.Faces[6] = new Face { A = 2, B = 3, C = 7 };
            mesh.Faces[7] = new Face { A = 3, B = 6, C = 7 };
            mesh.Faces[8] = new Face { A = 0, B = 2, C = 7 };
            mesh.Faces[9] = new Face { A = 0, B = 4, C = 7 };
            mesh.Faces[10] = new Face { A = 4, B = 5, C = 6 };
            mesh.Faces[11] = new Face { A = 4, B = 6, C = 7 };

            meshes[0] = mesh;


            camera.Position = new Vector3(0, 0, 10.0f);
            camera.Target = Vector3.Zero;

            // Registering to the XAML rendering loop
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            device.Clear(0, 0, 0, 255);

            foreach (var mesh in meshes)
            {
                // rotating slightly the meshes during each frame rendered
                //mesh.Rotation = new Vector3(mesh.Rotation.X + 0.01f, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);
            }

            // Doing the various matrix operations
            device.Render(camera, meshes);
            // Flushing the back buffer into the front buffer
            device.Present();
        }
    }
}
