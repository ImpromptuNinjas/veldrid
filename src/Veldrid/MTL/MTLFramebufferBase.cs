using System;
using Veldrid.MetalBindings;

namespace Veldrid.MTL
{
    internal abstract class MTLFramebufferBase : Framebuffer
    {
        protected abstract MetalBindings.MTLTexture GetMtlTexture(uint target);

        public MTLRenderPassDescriptor CreateRenderPassDescriptor(in RenderPassDescription rpi)
        {
             MTLRenderPassDescriptor ret = MTLRenderPassDescriptor.New();
            for (int i = 0; i < ColorTargets.Count; i++)
            {
                FramebufferAttachment colorTarget = ColorTargets[i];
                MTLRenderPassColorAttachmentDescriptor colorDescriptor = ret.colorAttachments[(uint)i];
                colorDescriptor.texture = GetMtlTexture((uint)i);
                colorDescriptor.loadAction = MTLFormats.VdToMTLLoadAction(rpi.LoadAction);
                colorDescriptor.storeAction = MTLFormats.VdToMTLStoreAction(rpi.StoreAction);
                colorDescriptor.slice = (UIntPtr)colorTarget.ArrayLayer;
                colorDescriptor.level = (UIntPtr)colorTarget.MipLevel;

                if (rpi.LoadAction == LoadAction.Clear)
                {
                    colorDescriptor.clearColor = new MTLClearColor(
                        rpi.ClearColor.R,
                        rpi.ClearColor.G,
                        rpi.ClearColor.B,
                        rpi.ClearColor.A);
                }
            }

            if (DepthTarget != null)
            {
                MTLTexture mtlDepthTarget = Util.AssertSubtype<Texture, MTLTexture>(DepthTarget.Value.Target);
                MTLRenderPassDepthAttachmentDescriptor depthDescriptor = ret.depthAttachment;
                depthDescriptor.loadAction = MTLFormats.VdToMTLLoadAction(rpi.LoadAction);
                depthDescriptor.storeAction = MTLFormats.VdToMTLStoreAction(rpi.StoreAction);
                depthDescriptor.texture = mtlDepthTarget.DeviceTexture;
                depthDescriptor.slice = (UIntPtr)DepthTarget.Value.ArrayLayer;
                depthDescriptor.level = (UIntPtr)DepthTarget.Value.MipLevel;

                if (rpi.LoadAction == LoadAction.Clear)
                {
                    depthDescriptor.clearDepth = rpi.ClearDepth;
                }

                if (FormatHelpers.IsStencilFormat(mtlDepthTarget.Format))
                {
                    MTLRenderPassStencilAttachmentDescriptor stencilDescriptor = ret.stencilAttachment;
                    stencilDescriptor.loadAction = MTLFormats.VdToMTLLoadAction(rpi.LoadAction);
                    stencilDescriptor.storeAction = MTLFormats.VdToMTLStoreAction(rpi.StoreAction);
                    stencilDescriptor.texture = mtlDepthTarget.DeviceTexture;
                    stencilDescriptor.slice = (UIntPtr)DepthTarget.Value.ArrayLayer;
                }
            }

            return ret;
        }

        public abstract bool IsRenderable { get; }

        public override string Name { get; set; }

        public MTLFramebufferBase(MTLGraphicsDevice gd, ref FramebufferDescription description)
            : base(description.DepthTarget, description.ColorTargets)
        {
        }

        public MTLFramebufferBase()
        {
        }
    }
}