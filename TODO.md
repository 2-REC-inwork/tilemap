# TODO

- [ ] FIX: When exporting Collision data, if it's same as Main Layer, need to limit values to number of collision "tiles".
    <br>(So that can have bigger tilesheet for the Main Layer, with more than 32 tiles - all tiles above 32 having no collision impact)

- [ ] When resetting a Layer, set empty cells to value of the last tile of the tilemap (instead of 255)
    (but how to handle when there's no tilemap ?)
    <br>=> OK like this, but not very good ...

- [ ] Look if smaller layers are aligned to bottom or top of main layer

- [ ] For the Objects & Hotspots layers, add controls (radio buttons) to choose what to display.
    <br>=> eg: "show all", "show same type", "show same"
