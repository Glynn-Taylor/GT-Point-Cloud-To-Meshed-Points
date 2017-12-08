Converts a ASCII PCL point cloud to triangles with uv'd texture for viewing in rendering software that can't render points or vertex colors.

Not currently designed for public use

Known limitations:
-currently only exports triangles
-If file is large enough then string builder will get too long for it's internal variables, causing a out of memory crash
-Exported 