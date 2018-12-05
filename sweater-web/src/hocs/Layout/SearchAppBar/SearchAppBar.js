import React from 'react';
import PropTypes from 'prop-types';
import {withStyles} from '@material-ui/core/styles';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import SearchInput from "../../../components/SearchInput/SearchInput";
import MenuItems from "./MenuItems/MenuItems";
import DesktopMenu from "./DesktopMenu/DesktopMenu";
import MobileMenu from "./MobileMenu/MobileMenu";

const styles = theme => ({
    root: {
        width: '100%',
    },
    grow: {
        flexGrow: 1,
    },
    menuButton: {
        marginLeft: -12,
        marginRight: 20,
    },
    title: {
        display: 'none',
        [theme.breakpoints.up('sm')]: {
            display: 'block',
        },
    }
});

class SearchAppBar extends React.Component {
    state = {
        anchorEl: null,
        mobileMoreAnchorEl: null,
    };

    onOpenAccountMenu = event => {
        this.setState({anchorEl: event.currentTarget});
    };

    onCloseDesktopMenu = () => {
        this.setState({anchorEl: null});
        this.onCloseMobileMenu();
    };

    onOpenMobileMenu = event => {
        this.setState({mobileMoreAnchorEl: event.currentTarget});
    };

    onCloseMobileMenu = () => {
        this.setState({mobileMoreAnchorEl: null});
    };

    render() {
        const {anchorEl, mobileMoreAnchorEl} = this.state;
        const {classes} = this.props;
        const isMenuOpen = Boolean(anchorEl);
        const isMobileMenuOpen = Boolean(mobileMoreAnchorEl);

        return (
            <>
                <AppBar
                    className={classes.root}
                    position={this.props.position}>
                    <Toolbar>
                        <Typography className={classes.title} variant="h6" color="inherit" noWrap>
                            {this.props.title}
                        </Typography>

                        <SearchInput onSearch={this.props.onSearch}/>

                        <div className={classes.grow}/>

                        <MenuItems
                            isMenuOpen={isMenuOpen}
                            onOpenAccountMenu={this.onOpenAccountMenu}
                            onOpenMobileMenu={this.onOpenMobileMenu}/>
                    </Toolbar>
                </AppBar>

                <DesktopMenu
                    anchorEl={anchorEl}
                    isOpen={isMenuOpen}
                    onClose={this.onCloseDesktopMenu}/>

                <MobileMenu
                    anchorEl={mobileMoreAnchorEl}
                    isOpen={isMobileMenuOpen}
                    onClose={this.onCloseMobileMenu}/>
            </>
        );
        // </div>
    }
}

SearchAppBar.propTypes = {
    classes: PropTypes.object.isRequired
    , position: PropTypes.string
};

export default withStyles(styles)(SearchAppBar);