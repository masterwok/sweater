import React from 'react';
import PropTypes from 'prop-types';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';


const MobileMenu = (props) => (
    <Menu
        anchorEl={props.anchorEl}
        anchorOrigin={{vertical: 'top', horizontal: 'right'}}
        transformOrigin={{vertical: 'top', horizontal: 'right'}}
        open={props.isOpen}
        onClose={props.onClose}>

        <MenuItem onClick={props.onClose}>Logout</MenuItem>
    </Menu>
);

MobileMenu.propTypes = {
    anchorEl: PropTypes.instanceOf(Element)
    , isOpen: PropTypes.bool.isRequired
    , onClose: PropTypes.func.isRequired
};

export default MobileMenu;
