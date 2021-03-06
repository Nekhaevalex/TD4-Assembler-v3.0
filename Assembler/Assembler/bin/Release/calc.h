#macro greetings {
    print "TD4E8_OS"
    out 10
}


//Commands:
//  r - run program from page n
//  s - shut down
//  h - help
//  b - boot again (reboot)

#macro strt_wrt {
    out '>'
}

#macro rp selected_val_addr caller {
    print "page_to_load:"

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
    print "1-help"
    out 10
    print "2-run"
    out 10
    print "3-reboot"
    out 10
    print ">=4-shutdown"
    out 10
}

#macro interpreter h_label r_label b_label s_label {
    mov a, 0
    mov b, 0
    strt_wrt
    getc selected
    ld selected
    mov a, b -48
    nop
    mov b, a
    gqt 4 s_label
    mov a, b
    gqt 3 b_label
    mov a, b
    gqt 2 r_label
    mov a, b
    gqt 1 h_label
    #undef selected
}

#macro main {
init:
    db 0 SELECTED_PROGRAM_PAGE
    greetings
cmd:
    interpreter help_label run_label boot_label shut_label
help_label:
    help
    jmp cmd
run_label:
    rp SELECTED_PROGRAM_PAGE cmd
    jmp cmd
boot_label:
    nop
    reboot init
shut_label:
    free SELECTED_PROGRAM_PAGE
    sd
    #undef SELECTED_PROGRAM_PAGE
}