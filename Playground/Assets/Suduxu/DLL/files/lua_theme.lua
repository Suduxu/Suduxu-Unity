return Column({
    SuduxuJoystick({
        size = 220,
        knobRatio = 0.45,
        padding = 12,
        baseColor = "#22000000",
        knobColor = "#FF888888",
        onInput = function(state)
            send_joystick_input(state)

            log("x=" .. state.x .. " y=" .. state.y ..
                    " angle=" .. state.angleDeg ..
                    " mag=" .. state.magnitude)
        end
    })
}, {

})