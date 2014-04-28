using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace TransparentWindow.Forms
{
    public partial class TestForm : FullScreenTransparentForm
    {
        public TestForm(Screen screen, ApplicationSettings settings)
            :base(screen)
        {
            // Init basic effect
            effect = new BasicEffect(GraphicsDevice);
        }

        // Directx graphics device    
        BasicEffect effect = null;     

        // Wheel vertexes
        VertexPositionColor[] v = new VertexPositionColor[100];

        // Wheel rotation
        float rot = 0;

        protected override void Draw()
        {
            base.Draw();

            // Rotate wheel a bit
            rot += 0.1f;

            // Make the wheel vertexes and colors for vertexes
            for (int i = 0; i < v.Length; i++)
            {
                if (i % 3 == 1)
                    v[i].Position = new Microsoft.Xna.Framework.Vector3((float)Math.Sin((i + rot) * (Math.PI * 2f / (float)v.Length)), (float)Math.Cos((i + rot) * (Math.PI * 2f / (float)v.Length)), 0);
                else if (i % 3 == 2)
                    v[i].Position = new Microsoft.Xna.Framework.Vector3((float)Math.Sin((i + 2 + rot) * (Math.PI * 2f / (float)v.Length)), (float)Math.Cos((i + 2 + rot) * (Math.PI * 2f / (float)v.Length)), 0);

                v[i].Color = new Microsoft.Xna.Framework.Color(1 - (i / (float)v.Length), i / (float)v.Length, 0, i / (float)v.Length);
            }

            // Enable position colored vertex rendering
            effect.VertexColorEnabled = true;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                pass.Apply();

            // Draw the primitives (the wheel)
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, v, 0, v.Length / 3, VertexPositionColor.VertexDeclaration);
        }
    }
}
