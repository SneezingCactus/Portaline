local trigger = {}

trigger.name = "Portaline/PortalineTrigger"
trigger.placements = {{
    name = "normal",
    data = {
        width = 16,
        height = 16,
        is_enable = true
    }
}, {
    name = "disable",
    data = {
        width = 16,
        height = 16,
        is_enable = false
    }
}}

return trigger
