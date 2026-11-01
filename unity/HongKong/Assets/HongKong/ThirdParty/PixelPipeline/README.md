# Pixel pipeline adoption

Source: https://github.com/bababuyyy/unity-isometric-pixel-pipeline/tree/f08bc6bf34b81c265171972610b23a8ef7c8d019 (MIT).

Adopted PixelRendererFeature and three PostProcess shaders only. Changes: project namespace and shader names; opt-in base Game camera marker; aspect-aware selectable resolution; explicit RenderGraph depth/normal and outline dependencies; no orthographic camera or global pan compensation. Full-resolution rendering still occurs before downsampling; this does not reduce geometry cost. UI is drawn after this pass. Per-camera mode is opt-in; renderer settings are not a shared gameplay controller.

Metal compiler fix: replace undefined equal-edge smoothstep(0.9,0.9,x) with step(0.9,x), guard angle cutoff denominator. Verified through the local build, not assumed from matching URP versions.
