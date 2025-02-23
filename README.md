# LSystem Visualizer
C# library for implementing various types of Lindenmayer systems, or L-systems. Primary resource is the first few chapters of https://www.algorithmicbotany.org/papers/abop/abop.pdf. 
All algorithmic work has been implemented from the ground up, including a custom AST Tree creator and parser for evaluation of parametric l-systems.

Includes support for
- Context free grammars such as `F → F-F+F+FF-F-F+F`
- Stochastic grammars, with weighted probabilities of choosing different production rules
- Context sensitive grammars, where replacement rules must also look at their neighbours for additional 'context' for the next state
- Parametric systems. These allow each symbol to have 1 or more numerical states, along with a set of variables and allows for algebraic manipulations.
  - An example would be

## 3D Tree
![image](https://github.com/user-attachments/assets/2ff2bdfd-329d-4011-9c64-64d89bbff0f1)

Example of 3D parametric cylinder made in unity, with the following grammar:
```
Starting word:
A(1,10)

Rules:
A(l,w) : -> !(w)F(l)[&(a)B(l*t,w*x)]/(d)A(l*r,w*x)
B(l,w) : -> !(w)F(l)[-(b)$C(l*t,w*x)]C(l*r,w*x)"
C(l,w) : -> !(w)F(l)[+(b)$B(l*t,w*x)]B(l*r,w*x)"

Values:
r = 0.9
t = rand(0.6, 0.9)
a = rand(30,60)
b = rand(30,60)
c = rand(90,180)
x = 0.707

```

## 2D Parametric
![Screenshot 2025-02-23 at 4 08 46 PM](https://github.com/user-attachments/assets/3a14501a-7665-4f84-96ec-f52321cc9303)

## Context Sensitive Grammar
![Screenshot 2025-02-23 at 4 09 28 PM](https://github.com/user-attachments/assets/3ce6bb2d-0ae0-4133-868a-4b231d583f91)

## Context Free Grammar
![Screenshot 2025-02-23 at 4 10 09 PM](https://github.com/user-attachments/assets/c079cf9d-5f52-4fa4-9919-f0d1c74e884e)
