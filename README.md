# Image Processing Algorithms

A Windows desktop application (C#, WPF) with **image processing algorithms implemented from scratch**:
point operations, thresholding, convolution filters, edge detection, morphology and geometric transformations.
It was built during the *Digital Image Processing* lab, one worksheet per week.

Every algorithm works directly on the pixel data. Emgu CV (an OpenCV wrapper) is used only to load, store and
display images, never for the processing itself. All operations support both grayscale and color (BGR) images unless
noted otherwise.

## Implemented algorithms

### Point operations
| Algorithm | Description |
|---|---|
| **Brightness & contrast** | Linear transform `g = a·f + b` through a 256-entry lookup table (LUT), clamped to [0, 255] |
| **Gamma correction** | Non-linear transform `g = c·f^γ`, with `c` chosen so that 255 maps to 255; implemented with a LUT |

### Thresholding
| Algorithm | Description |
|---|---|
| **Otsu thresholding** | Computes the histogram and picks the threshold that maximizes the between-class variance, then binarizes the image. Color images are first converted to grayscale (`0.299 R + 0.587 G + 0.114 B`) |

### Filters (convolution)
| Algorithm | Description |
|---|---|
| **Generic convolution** | Applies any `m × n` kernel, with mirrored borders (reflect-101) so the output keeps the input size |
| **Low-pass (weighted mean)** | 3×3 smoothing kernel with a heavier center weight |
| **Separable filter** | Smoothing with the 1D kernel `[1 2 1] / 4` applied horizontally, then vertically: equivalent to the 3×3 kernel `[1 2 1; 2 4 2; 1 2 1] / 16`, with 6 instead of 9 multiplications per pixel |
| **High-pass (sharpening)** | 3×3 Laplacian-based sharpening kernel `[0 -1 0; -1 5 -1; 0 -1 0]` |
| **Gaussian kernel** | Generates a normalized `size × size` Gaussian mask for any σ |
| **Unsharp masking** | Sharpens by adding back the detail removed by a 5×5 Gaussian blur (σ = 1): `g = 2f − f_low` |

### Edge detection
| Algorithm | Description |
|---|---|
| **Directional Sobel edge detection** | Computes the gradient with the Sobel operators, keeps pixels whose gradient magnitude exceeds a threshold `T`, and only those whose edge direction is within `±dev` degrees of a chosen angle `θ` (e.g. only horizontal or only vertical edges) |

### Morphological operations
| Algorithm | Description |
|---|---|
| **Connected components labeling** | Binarizes the image with Otsu, then finds every 8-connected object with a breadth-first flood fill and paints each one in a random color (grayscale input) |

### Geometric transformations
| Algorithm | Description |
|---|---|
| **Scaling with bilinear interpolation** | Resizes by independent factors `sx`, `sy` using inverse mapping: each output pixel is interpolated from its 4 nearest input pixels |
| **Mirroring** | Horizontal flip |
| **Rotation by 90°** | Clockwise and anti-clockwise |
| **Crop with statistics** | Crops the region selected with two mouse clicks and reports its mean and standard deviation (grayscale input) |

## Worksheets
| Worksheet | Topic | What was implemented |
|---|---|---|
| Labs 1–5 | Image basics, point operations, thresholding, filtering | mirroring, 90° rotations, crop with mean / std, brightness & contrast, gamma correction, Otsu thresholding, generic convolution, low-pass, separable and high-pass filters |
| Unsharp mask | Sharpening | Gaussian kernel generation, unsharp masking |
| Week 7 | High-pass filtering, edges | directional Sobel edge detection |
| Lab 11 | Morphological operations II | connected components labeling |
| Lab 12 | Geometric transformations | scaling with bilinear interpolation |

## Running the application
Requirements: **Windows** and **Visual Studio 2022** with the *.NET desktop development* workload (.NET Framework 4.8).

1. Clone the repository and open `ImageProcessing.sln`.
2. Build the solution: the NuGet packages (Emgu CV, OxyPlot, OpenTK) are restored automatically.
3. Run the `Framework` project.
4. **File → Load grayscale image** or **Load color image**, then pick an operation from the menu.
   The original image is shown on the left and the result on the right.

Useful tools that come with the application: a magnifier, row / column color level plots, histograms of the original
and processed images, and **Use processed image as initial image** to chain several operations.

## Project structure
```
Algorithms/                     image processing library
├── Sections/
│   ├── PointwiseOperations.cs    brightness & contrast, gamma, LUTs
│   ├── Thresholding.cs           Otsu thresholding
│   ├── Filters.cs                convolution, Gaussian kernel, unsharp mask, Sobel
│   ├── MorphologicalOperations.cs connected components
│   └── GeometricTransformations.cs bilinear scaling
└── Tools/Tools.cs              mirroring, rotation, crop, statistics
Framework/                      WPF user interface (menus, dialogs, image viewers)
└── ViewModel/Commands/MenuCommands.cs   connects every menu entry to its algorithm
```

## Credits
The WPF application shell (image viewers, dialogs, histogram and magnifier windows) is the framework provided for
the lab and is distributed under the LGPL (see `License-LGPL.txt`). The algorithms in `Algorithms/` and their menu
commands were implemented by **Nicolas Crăciun** ([@craciunnicolas10](https://github.com/craciunnicolas10)).

## Tech stack
C# · .NET Framework 4.8 · WPF · Emgu CV (image I/O only)
