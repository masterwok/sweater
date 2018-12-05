import React from 'react';
import PropTypes from 'prop-types';
import {withStyles} from '@material-ui/core/styles';
import Button from '@material-ui/core/Button';
import Snackbar from '@material-ui/core/Snackbar';
import IconButton from '@material-ui/core/IconButton';
import CloseIcon from '@material-ui/icons/Close';
import ErrorIcon from '@material-ui/icons/Error';
import Icon from "@material-ui/core/Icon/Icon";


const styles = theme => ({
    close: {
        padding: theme.spacing.unit / 2,
    },
    error: {
        backgroundColor: theme.palette.error.dark,
    },
    iconVariant: {
        opacity: 0.9,
        marginRight: theme.spacing.unit,
    },
    message: {
        display: 'flex',
        alignItems: 'center',
    }
});

class ErrorSnack extends React.Component {
    state = {
        open: false,
    };

    handleClick = () => {
        this.setState({open: true});
    };

    handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }

        this.setState({open: false});
    };

    render() {
        const {classes} = this.props;
        return (
            <div>
                <Button onClick={this.handleClick}>Open simple snackbar</Button>
                <Snackbar
                    anchorOrigin={{
                        vertical: 'bottom',
                        horizontal: 'left',
                    }}
                    open={this.state.open}
                    autoHideDuration={6000}
                    onClose={this.handleClose}
                    ContentProps={{
                        'aria-describedby': 'message-id',
                    }}
                    message={
                        <span id="client-snackbar" className={classes.message}>
                          <ErrorIcon className={[classes.icon, classes.iconVariant].join(' ')}/>
                            Hack the planet!!
                        </span>
                    }
                    action={[
                        <IconButton
                            key="close"
                            aria-label="Close"
                            color="inherit"
                            className={classes.close}
                            onClick={this.handleClose}
                        >
                            <CloseIcon/>
                        </IconButton>,
                    ]}
                />
            </div>
        );
    }
}

ErrorSnack.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(ErrorSnack);