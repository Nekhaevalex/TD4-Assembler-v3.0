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
    print "h-help"
    out 10
    print "r-run"
    out 10
    print "b-reboot"
    out 10
    print "s-shutdown"
    out 10
}

#macro interpreter h_label r_label b_label s_label{
    strt_wrt
    getc
    mov b, a -48
    
}

#macro main {
init:
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