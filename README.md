# Talk
This is where I store all my talk material.
## Scaffold New Talk
- Slides generated using [FsReveal](https://github.com/kimsk/FsReveal)
- 'init.cmd' to downlaod and install dependencies
- 'build.cmd' to gen slides to 'output' folder
    - Use 'build.cmd KeepRunning' to live edit slides

## Run An Existing Demo
- 'init.cmd' to downlaod and install dependencies
- Open solution

## Scaffold New Demo
- With [Paket](https://github.com/fsprojects/Paket)
- Copy the scaffold directory and rename as needed
- Add dependencies and references to 
   - paket.dependencies
   - paket.references
- Create demo solution
- Run ./paket/paket.exe install --hard
- Update .gitignore