import React from 'react';
import PropTypes from 'prop-types';
import {withStyles} from '@material-ui/core/styles';
import LinearProgress from '@material-ui/core/LinearProgress';

const styles = {
    root: {
        flexGrow: 1,
    },
};

const ProgressBar = (props) => {
    return props.isLoading ?
        <LinearProgress color="secondary" variant='query'/>
        : null;
};

ProgressBar.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(ProgressBar);