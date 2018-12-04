import React from 'react';
import PropTypes from 'prop-types';
import {withStyles} from '@material-ui/core/styles';
import LinearProgress from '@material-ui/core/LinearProgress';

const styles = {
    root: {
        flexGrow: 1,
    },
};

function LinearIndeterminate(props) {
    return (
        <LinearProgress color="secondary"/>
    );
}

LinearIndeterminate.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(LinearIndeterminate);