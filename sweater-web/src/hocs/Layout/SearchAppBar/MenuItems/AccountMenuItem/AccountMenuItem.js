import React from 'react';
import PropTypes from 'prop-types';
import IconButton from '@material-ui/core/IconButton';
import AccountCircle from '@material-ui/icons/AccountCircle';


const AccountMenuItem = (props) => (
    <IconButton
        aria-owns={props.isMenuOpen ? 'material-appbar' : undefined}
        aria-haspopup="true"
        onClick={props.onClick}
        color="inherit">
        <AccountCircle/>
    </IconButton>
);


AccountMenuItem.propTypes = {
    isMenuOpen: PropTypes.bool.isRequired
};

export default AccountMenuItem;
