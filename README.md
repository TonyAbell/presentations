# Talk
This is where I store all my talk material.
## Scaffold New Talk
- Slides generated using [FsReveal](https://github.com/kimsk/FsReveal)
- Copy the scaffold directory and rename as needed
- run ./paket/paket.exe install
   - will install packages
- Fix Bug in FsReveal Package
    - Edit packages\FsReveal\fsreveal\fsreveal.fsx
    - Remove Version Info from 
        - #I @"..\..\FSharp.Formatting\lib\net40\"
        - #I @"..\..\FSharp.Compiler.Service\lib\net40\"
- Edit ./slides/slides.md as needed
- 'Build.cmd' to gen slides to 'output'
    - Use 'Build.cmd KeepRunning' to live edit slides

## Scaffold New Demo
- With [Paket](https://github.com/fsprojects/Paket)
- Copy the scaffold directory and rename as needed
- Add dependencies and references to 
   - paket.dependencies
   - paket.references
- Create demo solution
- Run ./paket/paket.exe install --hard