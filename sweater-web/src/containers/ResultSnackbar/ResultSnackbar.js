import React from 'react';
import PropTypes from 'prop-types';
import Snackbar from '@material-ui/core/Snackbar';
import {withStyles} from '@material-ui/core/styles';
import SnackbarResultContent from "./SnackbarResultContent/SnackbarResultContent";


class ResultSnackbar extends React.Component {
    state = {
        open: false
        , variant: 'success'
        , message: null
    };

    showError = (message) => {
        this.setState({
            open: true
            , variant: 'error'
            , message: message
        });
    };


    handleClose = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }

        this.setState({open: false});
    };

    render() {
        return (
            <Snackbar
                anchorOrigin={{
                    vertical: 'bottom',
                    horizontal: 'left',
                }}
                open={this.state.open}
                autoHideDuration={6000}
                onClose={this.handleClose}>
                <SnackbarResultContent
                    onClose={this.handleClose}
                    variant={this.state.variant}
                    message={this.state.message}/>
            </Snackbar>
        );
    }
}

ResultSnackbar.propTypes = {
    // classes: PropTypes.object.isRequired,
};

export default ResultSnackbar;
