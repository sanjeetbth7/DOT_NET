## .NET
The .NET framework is a software development platform developed by Microsoft that provides a controlled environment for building and running applications. It is developed as competitor of java.

### C# (C Sharp)
C# is type safe object oriented programming language developed by Mircrosoft within the .NET framework.
It is designed for building a variety of applications that run on the .NET ecosystem, including web, desktop, mobile, and cloud-based applications.

>C# is platform-independent, provided it is used with a modern cross-platform runtime.

>Developed in 1999 by pricipal designer and lead architect of Microsoft Anders Hejlsberg.

Key Features of C#:
1. Object-Oriented: C# supports the principles of object-oriented programming, including encapsulation, inheritance, and polymorphism.
2. Type Safety: C# is a statically typed language, which helps catch type-related errors at compile time rather than at runtime.
3. Memory Management: C# includes automatic memory management through garbage collection, reducing the risk of memory leaks.
4. Language Integrated Query (LINQ): C# provides built-in support for querying collections in a more readable and concise manner using LINQ.
5. Asynchronous Programming: C# supports asynchronous programming with the async and await keywords, making it easier to write responsive applications.
6. Rich Standard Library: C# has a comprehensive standard library that provides a wide range of functionalities, from file I/O to networking.

#### *Design Goals*
1. Simplicity: C# was designed to be simple and easy to learn, with a syntax that is familiar to developers who have experience with C-style languages.
2. Modernity: C# incorporates modern programming concepts and features to facilitate the development of contemporary applications.
3. Versatility: C# is designed to be versatile, allowing developers to build a wide range of applications, from web and mobile apps to games and enterprise software.
4. Performance: C# aims to provide high performance through features like just-in-time (JIT) compilation and optimizations in the .NET runtime.
5. Interoperability: C# is designed to work seamlessly with other languages and technologies, particularly within the .NET ecosystem.
6. Safety and Security: C# emphasizes type safety and includes features to help prevent common programming errors, enhancing the security of applications.
7. Productivity: C# includes features that enhance developer productivity, such as strong tooling support, integrated development environments (IDEs), and a rich set of libraries and frameworks.

#### *How it compile and run*

C# code --compilation [common language runtime(CLR)]--> Intermedeate Language (IL) code --JIT compilation--> Machine Code


first compiled and then interpreted

C# code is typically compiled and run using the following steps:
1. Writing Code: Developers write C# code using a text editor or an integrated development environment (IDE) such as Visual Studio.
2. Compilation: The C# code is compiled using the C# compiler (csc.exe) into an intermediate language (IL) code, which is stored in an assembly (DLL or EXE file).
3. Just-In-Time (JIT) Compilation: When the application is executed, the .NET runtime's Just-In-Time (JIT) compiler converts the IL code into native machine code specific to the operating system and hardware.
4. Execution: The native machine code is executed by the operating system, allowing the application to run.
5. Garbage Collection: During execution, the .NET runtime manages memory allocation and deallocation through garbage collection, automatically reclaiming memory that is no longer in use.
#### *Hello World Example*

```csharp
using System;
class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");
    }
}
```
This simple C# program prints "Hello, World!" to the console. It demonstrates the basic structure of a C# application, including the use of namespaces, classes, and the Main method as the entry point of the program.
### .NET Core vs .NET Framework
- **.NET Framework**: The original implementation of .NET, primarily for building Windows desktop and server applications. It is a Windows-only framework and has been around since the early 2000s.
- **.NET Core**: A cross-platform, open-source implementation of .NET that allows developers to build applications that run on Windows, macOS, and Linux. .NET Core was introduced in 2016 to address the need for a more modular and flexible framework.

#### Advantages
- **Cross-Platform**: .NET Core supports multiple operating systems, making it suitable for a wider range of applications.
- **Interoperability**: "Interop" process enables C# programs to do almost anything that a native C++ application can do.
