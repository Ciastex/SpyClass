// yes i stole it from w3c
// no i don't care.
/*
*   This content is licensed according to the W3C Software License at
*   https://www.w3.org/Consortium/Legal/2015/copyright-software-and-document
*
*   File:   TreeitemLink.js
*
*   Desc:   Treeitem widget that implements ARIA Authoring Practices
*           for a tree being used as a file viewer
* 
* Changes:
* - removed keyboard input support
* - minor code cleanups
*/

var TreeitemLink = function (node, treeObj, group) {
  if (typeof node !== 'object') {
    return;
  }

  node.tabIndex = -1;
  this.tree = treeObj;
  this.groupTreeitem = group;
  this.domNode = node;
  this.label = node.textContent.trim();
  this.stopDefaultClick = false;

  if (node.getAttribute('aria-label')) {
    this.label = node.getAttribute('aria-label').trim();
  }

  this.isExpandable = false;
  this.isVisible = false;
  this.inGroup = !!group;

  var elem = node.firstElementChild;

  while (elem) {

    if (elem.tagName.toLowerCase() === 'ul') {
      elem.setAttribute('role', 'group');
      this.isExpandable = true;
      break;
    }

    elem = elem.nextElementSibling;
  }
};

TreeitemLink.prototype.init = function () {
  this.domNode.tabIndex = -1;

  if (!this.domNode.getAttribute('role')) {
    this.domNode.setAttribute('role', 'treeitem');
  }

  this.domNode.addEventListener('click', this.handleClick.bind(this));
  this.domNode.addEventListener('focus', this.handleFocus.bind(this));
  this.domNode.addEventListener('blur', this.handleBlur.bind(this));

  if (this.isExpandable) {
    this.domNode.firstElementChild.addEventListener('mouseover', this.handleMouseOver.bind(this));
    this.domNode.firstElementChild.addEventListener('mouseout', this.handleMouseOut.bind(this));
  }
  else {
    this.domNode.addEventListener('mouseover', this.handleMouseOver.bind(this));
    this.domNode.addEventListener('mouseout', this.handleMouseOut.bind(this));
  }
};

TreeitemLink.prototype.isExpanded = function () {

  if (this.isExpandable) {
    return this.domNode.getAttribute('aria-expanded') === 'true';
  }

  return false;

};

TreeitemLink.prototype.handleClick = function (event) {
  if (event.target !== this.domNode && event.target !== this.domNode.firstElementChild) {
    return;
  }

  if (this.isExpandable) {
    if (this.isExpanded()) {
      this.tree.collapseTreeitem(this);
    }
    else {
      this.tree.expandTreeitem(this);
    }
    event.stopPropagation();
  }
};

TreeitemLink.prototype.handleFocus = function (event) {
  var node = this.domNode;
  if (this.isExpandable) {
    node = node.firstElementChild;
  }
  node.classList.add('focus');
};

TreeitemLink.prototype.handleBlur = function (event) {
  var node = this.domNode;
  if (this.isExpandable) {
    node = node.firstElementChild;
  }
  node.classList.remove('focus');
};

TreeitemLink.prototype.handleMouseOver = function (event) {
  event.currentTarget.classList.add('hover');
};

TreeitemLink.prototype.handleMouseOut = function (event) {
  event.currentTarget.classList.remove('hover');
};

