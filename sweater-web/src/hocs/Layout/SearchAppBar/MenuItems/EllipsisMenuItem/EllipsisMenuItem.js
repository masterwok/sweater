import React from 'react';
import PropTypes from 'prop-types';
import IconButton from '@material-ui/core/IconButton';
import MoreIcon from '@material-ui/icons/MoreVert';


const EllipsisMenuItem = (props) => (
    <IconButton
        aria-haspopup="true"
        onClick={props.onClick}
        color="inherit">
        <MoreIcon/>
    </IconButton>
);

EllipsisMenuItem.propTypes = {
    onClick: PropTypes.func.isRequired
};

export default EllipsisMenuItem;
