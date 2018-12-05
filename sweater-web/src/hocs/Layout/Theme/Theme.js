import {createMuiTheme} from '@material-ui/core/styles';


const theme = createMuiTheme({
        typography: {
            useNextVariants: true
        }
        , palette: {
            primary: {
                main: '#2b393d',
                light: '#546e67',
                dark: '#011317',
                contrastText: '#fff',
            },
            secondary: {
                main: '#008dad',
                light: '#56bddf',
                dark: '#005f7e',
                contrastText: '#000',
            }
        }
    })
;

export default theme;