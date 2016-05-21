//set all items with a non empty string as their title to use qtip
$('[title!=""]').qtip({
    position: {
        my: 'center right',
        at: 'center right',
        //target: 'mouse',
        adjust: {
            mouse: false 
        }
    },
    hide: {
        fixed: false,
        event: 'unfocus click mouseleave',
        delay: 0,
        leave: true
    },
    show: {
        delay: 1000
    },
    style: {
        classes: 'qtip-white qtip-shadow qtip-bootstrap'
    }
});

$('.action-button').qtip({
    content:{
        text:'When it is your turn, choose a board location to move to by clicking this area.',
        title: 'Move Player',
    },
    position: {
        my: 'center left',
        at: 'center right',
        adjust: {
            mouse: false,
            x: 10
        }
    },
    hide: {
        fixed: true, //prevent from disappearing if you hover over it
        event: 'unfocus mouseleave',
        delay: 0,
        leave: true
    },
    show: {
        delay: 2000 //wait two seconds before showing help. We don't want to spam helps
    },
    style: {
        classes: 'qtip-white qtip-shadow qtip-bootstrap default-text-color' //might want to choose different styles for help tooltips
    }
});

//this is a generic tooltip I am setting up that can be used for displaying help info on game board
$('.help-tooltip').qtip({
    content: {
        text: function () {
            return $(this).attr('data-text');
        },
        title: function () {
            return $(this).attr('data-title');
        },
    },
    position: {
        my: 'center left',
        at: 'center right',
        adjust: {
            mouse: false,
            x: 10
        }
    },
    hide: {
        fixed: true, //prevent from disappearing if you hover over it
        event: 'unfocus mouseleave',
        delay: 0,
        leave: true
    },
    show: {
        delay: 2000 //wait two seconds before showing help. We don't want to spam helps
    },
    style: {
        classes: 'qtip-white qtip-shadow qtip-bootstrap default-text-color' //might want to choose different styles for help tooltips
    }
});