#import std.h

#macro greetings {
    #define HELLO_WORLD_STRING_ADDRESS 0x0
    #define str "TD4E8_OS"
    db_s str HELLO_WORLD_STRING_ADDRESS HEL_LENGTH
    out_addr HELLO_WORLD_STRING_ADDRESS HEL_LENGTH
    #undef str
    #undef HEL_LENGTH
    #undef HELLO_WORLD_STRING_ADDRESS
    out 10
}


//Commands:
//  r - run program from page n
//  s - shut down
//  h - help
//  b - boot again (reboot)

#macro strt_wrt {
    out 10
    out '>'
}

#macro rp selected_val_addr caller {
    
    //Return
    jmp caller
}

#macro sd {
    hlt
}

#macro reboot init_label {
    jmp init_label
}

#macro help {
    #define HELP_STR  0x8
    #define help_str "h|r|b|s"
    db_s help_str HELP_STR HELP_STR_LEN
    out_addr HELP_STR help_str.length
    #undef help_str
    #undef help_str.length
    #undef HELP_STR
    #undef HELP_STR_LEN
}

#macro interpreter h_label r_label b_label s_label{
    strt_wrt

}

#macro main {
init:
    #define SELECTED_PROGRAM_PAGE 0x10
    db 0 SELECTED_PROGRAM_PAGE
    greetings
cmd:
    interpreter help_label run_label boot_label shut_label
help_label:
    help
run_label:
    rp SELECTED_PROGRAM_PAGE cmd
boot_label:
    reboot init
shut_label:
    sd
    #undef SELECTED_PROGRAM_PAGE
}